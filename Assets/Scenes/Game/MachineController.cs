using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Obi;
using Grabby;
using Grabby.UI;

[Serializable]
public struct MachineControlState
{
    public bool isPressed;
    public float setX;
    public float setZ;
    public float fixedDeltaTime;
}
internal class DebugTimer
{
    public float value;
}

public class MachineController : MonoBehaviour
{
    [SerializeField] private ModalOpener negativeCardOpener;
    [SerializeField] private Transform moverByX;
    [SerializeField] private Transform moverByZ;
    [SerializeField] private Transform tentacle1;
    [SerializeField] private Transform tentacle2;
    [SerializeField] private Transform tentacle3;
    [SerializeField] private Transform centerOfTentacles;
    [SerializeField] private Transform iron;
    [SerializeField] private TentaclesCollider tentaclesCollider;
    [SerializeField] public LoadsController loadsController;
    [SerializeField] private TimerController timer;
    [SerializeField] private WinCollider winCollider;
    [SerializeField] private Animator standForButtonAnimator;
    [SerializeField] private ObiRope rope;
    [SerializeField] private ObiRopeCursor ropeCursor;
    [SerializeField] public ObiCustomUpdater ropeUpdater;
    [SerializeField] private string toysLayerName = "Toy";
    [SerializeField] private float moverDefaultPositionX = 0.45f;
    [SerializeField] private float moverDefaultPositionZ = 0f;
    [SerializeField] private float moverMoveToDefaultDuration = 2f;
    [SerializeField] private float moverByXMin = 0.1002f;
    [SerializeField] private float moverByXMax = 0.7956f;
    [SerializeField] private float moverByZMin = -0.536f;
    [SerializeField] private float moverByZMax = 0.536f;
    [SerializeField] private float ropeGrabbyLength = 0.9f;
    [SerializeField] private float ropeGrabbyDuration = 3f;
    [SerializeField] private float ropeDefaultDuration = 2f;
    [SerializeField] private float tentaclesOpenDuration = 0.6f;
    [SerializeField] private float tentaclesCloseDuration = 1f;

    // контроллер
    [NonSerialized] public bool busy = false;
    [NonSerialized] public bool movingToDefault = false;
    [NonSerialized] internal Dictionary<int, Load> wonLoads = new Dictionary<int, Load>();
    private float moveByX = 0f;
    private float moveByZ = 0f;
    private bool gameStarting = false;
    private bool gameStarted = false;
    private bool gameEnding = false;
    private string gameType = null;
    private bool canRelease = false;
    private float ropeDefaultLength;
    private bool stopWhenCollidingLeftStarted = false;
    private float stopWhenCollidingLeft = 0f;
    private bool isBoardPressed = false;
    private List<MachineControlState> controlState = new List<MachineControlState>();
    private int toysLayer;

    // симуляция
    private bool simulation = false;
    private float currentFixedDeltaTime;
    private float customFixedDeltaTime => simulation == true ? currentFixedDeltaTime : Time.fixedDeltaTime;
    private List<TaskCompletionSource<bool>> waitCustomFixedUpdate = new List<TaskCompletionSource<bool>>();
    private List<TaskCompletionSource<bool>> waitCustomForSeconds = new List<TaskCompletionSource<bool>>();
    private Dictionary<TaskCompletionSource<bool>, float> waitCustomForSecondsDict = new Dictionary<TaskCompletionSource<bool>, float>();
    private PhysicsScene physicsScene = Physics.defaultPhysicsScene;
    private List<DebugTimer> timers = new List<DebugTimer>();

    // ***
    //
    // события юнити
    //
    // ***
    private void Awake() {
        ropeDefaultLength = rope.restLength;
        toysLayer = 1 << LayerMask.NameToLayer(toysLayerName);
    }
    private void Start() {
        if(simulation == false) {
            loadsController.Generate();
        }
    }
    private void Update() {
        // начинаем игру если двинули клешнёй
        if(
            simulation == false &&
            gameStarting == false &&
            gameStarted == false &&
            busy == false &&
            movingToDefault == false &&
            loadsController.antiWinColliderShowed == false &&
            (moveByX != 0f || moveByZ != 0f)
        ) {
            StartGame();
        }

        // делаем кнопку синей
        standForButtonAnimator.SetBool("isBlue", canRelease);
    }
    private void FixedUpdate() {
        // двигаем клешню в зависимости от moveByX и moveByZ
        if(movingToDefault == false && simulation == false) {
            if(moveByX != 0f) {
                SetX(Mathf.Clamp(moverByX.localPosition.x - moveByX * customFixedDeltaTime, moverByXMin, moverByXMax));
            }
            if(moveByZ != 0f) {
                SetZ(Mathf.Clamp(moverByZ.localPosition.z + moveByZ * customFixedDeltaTime, moverByZMin, moverByZMax));
            }
        }
        // сохраняем состояние игры в этом кадре для симуляции на сервере
        if(gameStarted == true && gameEnding != true && simulation != true) {
            MachineControlState state;
            state.isPressed = isBoardPressed;
            state.setX = moverByX.localPosition.x;
            state.setZ = moverByZ.localPosition.z;
            state.fixedDeltaTime = Time.fixedDeltaTime;
            controlState.Add(state);
        }
        // для сравнения таймера в симуляции и продакшене
        if(simulation == false) {
            Step();
        }
    }

    // ***
    //
    // следующие методы посвящены симуляции
    //
    // ***
    public void SetSimulate(PhysicsScene scene) {
        simulation = true;
        gameStarted = true;
        physicsScene = scene;
    }
    // устанавливаем текущее состояние игры в текущем кадре
    public void SetState(MachineControlState state) {
        if(state.isPressed) {
            OnBoardPress();
        }
        SetX(state.setX);
        SetZ(state.setZ);
        currentFixedDeltaTime = state.fixedDeltaTime;
    }
    // кастомная функция WaitForFixedUpdate
    // так как new WaitForFixedUpdate работает зависимо от FixedUpdate
    // который не вызывается при Physics.Simulate
    // приходится реализовывать собственный метод
    async private Task CustomWaitForFixedUpdate() {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        waitCustomFixedUpdate.Add(tcs);
        await tcs.Task;
    }
    // кастомная функция WaitForSeconds
    // так как new WaitForSeconds работает зависимо от FixedUpdate
    // который не вызывается при Physics.Simulate
    // приходится реализовывать собственный метод
    async private Task CustomWaitForSeconds(float duration) {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        waitCustomForSeconds.Add(tcs);
        waitCustomForSecondsDict.Add(tcs, duration);
        await tcs.Task;
    }
    // метод который заменяет FixedUpdate в моей симуляции
    public void Step() {
        // состояние isBoardPressed в одном кадре должно быть активно при нажатии на кнопку
        // после выхода из кадра устанавливаю его на false
        if(isBoardPressed == true) {
            isBoardPressed = false;
        }

        // выполняю CustomWaitForFixedUpdate
        foreach(var tcs in waitCustomFixedUpdate.ToList()) {
            tcs.TrySetResult(true);
            waitCustomFixedUpdate.Remove(tcs);
        }

        // выполняю CustomWaitForSeconds
        foreach(var key in waitCustomForSecondsDict.Keys.ToList()) {
            float duration = waitCustomForSecondsDict[key];
            duration -= customFixedDeltaTime;
            if(duration <= 0) {
                key.TrySetResult(true);
                waitCustomForSeconds.Remove(key);
                waitCustomForSecondsDict.Remove(key);
            }
            else {
                waitCustomForSecondsDict[key] = duration;
            }
        }
        
        // таймеры
        foreach(var timer in timers) {
            timer.value += customFixedDeltaTime;
        }
    }
    // метод возвращающий данные текущего кадра сцены для сервера
    public GameScene GetCurrentGameScene() {
        GameScene scene;
        scene.loads = loadsController.SetGameSceneLoads().ToArray();
        scene.ironPosition = iron.position;
        scene.ironRotation = iron.eulerAngles;
        return scene;
    }
    // метод устанавливающий текущую сцену в позиции сохранённые на сервере
    public void SetCurrentGameScene(GameScene scene) {
        loadsController.SetUp(scene);
        iron.position = scene.ironPosition;
        iron.eulerAngles = scene.ironRotation;
    }
    // дебаг таймер
    private DebugTimer CreateTimer() {
        DebugTimer timer = new DebugTimer();
        timers.Add(timer);
        return timer;
    }

    // ***
    //
    // метод начала и окончания игры
    //
    // ***
    async private Task StartGame() {
        gameStarting = true;
        try {
            // gameType = Store.settings.liveGame ? "live" : "test";

            // APIGameStartedResponse response = await API.GameStarted(gameType, GetCurrentGameScene());

            // User user = Store.user;
            // if(gameType == "live") {
            //     user.game.liveTries = response.leftTries;
            // }
            // else {
            //     user.game.testTries = response.leftTries;
            // }
            // Store.user = user;

            // GameSceneSetUp.currentGameScene.ShowTries();
            timer.StartGrabby();
            gameStarted = true;
        } catch(Exception e) {
            Modal modal = negativeCardOpener.OpenAndClose();
            modal.GetComponent<NegativeCard>().SetDescription(e.Message);
        }
        gameStarting = false;
    }
    async private Task EndGame() {
        // try {
        //     APIGameEndedResponse response = await API.GameEnded(
        //         gameType,
        //         controlState,
        //         GetCurrentGameScene(),
        //         wonLoads.Select(wonLoad => wonLoad.Value.name).ToArray()
        //     );

        //     User user = Store.user;
        //     user.game = response.userGame;
        //     Store.user = user;

        //     SetCurrentGameScene(response.endScene);

        //     foreach(string wonLoad in response.simulatedWonLoads) {
        //         winCollider.ShowCaughtLoadDialog(wonLoad);
        //     }
        //     winCollider.SetData(response.newScore, response.newCoins);

        // } catch(Exception e) {

        // }
    }
    public void ClearStates() {
        wonLoads.Clear();
        controlState.Clear();
    }

    // ***
    //
    // метод нажатия на кнопку
    //
    // ***
    async public Task OnBoardPress() {
        isBoardPressed = true;

        if(canRelease) {
            await Release();
        }
        else {
            await Grab();
        }
    }
    // ***
    //
    // публичный метод который вызывается когда игрушка падает с клешни
    // 
    // ***
    async public Task OnToyDropped() {
        canRelease = false;
        timer.ShowCurrentTries();
        await DefaultTentacles(tentaclesCloseDuration);
        await MoveToDefault(moverMoveToDefaultDuration);
    }

    // ***
    //
    // методы для передвижения клешни
    //
    // ***
    public void MoveX(float delta) {
        moveByX = delta;
    }
    public void MoveZ(float delta) {
        moveByZ = delta;
    }
    public void Unmove() {
        moveByX = 0f;
        moveByZ = 0f;
    }
    async private Task MoveToDefault(float duration) {
        if(movingToDefault == true || gameStarted == false) return;

        movingToDefault = true;

        if(simulation == true) {
            await CustomWaitForSeconds(duration);
        }
        else {
            float positionX = moverByX.localPosition.x;
            float positionZ = moverByZ.localPosition.z;

            float passed = 0f;
            while(passed < duration) {
                passed += customFixedDeltaTime;

                float progress = passed / duration;

                SetX(Mathf.Lerp(positionX, moverDefaultPositionX, progress));
                SetZ(Mathf.Lerp(positionZ, moverDefaultPositionZ, progress));

                await CustomWaitForFixedUpdate();
            }

            SetX(moverDefaultPositionX);
            SetZ(moverDefaultPositionZ);

            gameEnding = true;

            await EndGame();
            ClearStates();

            gameEnding = false;
        }

        movingToDefault = false;
        gameStarted = false;
        gameType = null;
    }
    private void SetX(float value) {
        if(gameStarted == false) return;
        Vector3 position = moverByX.localPosition;
        position.x = value;
        moverByX.localPosition = position;
    }
    private void SetZ(float value) {
        if(gameStarted == false) return;
        Vector3 position = moverByZ.localPosition;
        position.z = value;
        moverByZ.localPosition = position;
    }

    // ***
    //
    // методы захвата игрушек и отпускания игрушек
    //
    // ***
    async public Task Grab() {
        if(busy == true || gameStarted == false) return;

        busy = true;
        timer.StartRelease();

        DebugTimer openTentaclesTimer = CreateTimer();
        await OpenTentacles(tentaclesOpenDuration);

        DebugTimer changeRopeLengthTimer = CreateTimer();
        await ChangeRopeLength(ropeGrabbyLength, ropeGrabbyDuration, true);

        DebugTimer closeTentaclesTimer = CreateTimer();
        await CloseTentacles(tentaclesCloseDuration);

        DebugTimer changeRopeLength2Timer = CreateTimer();
        await ChangeRopeLength(ropeDefaultLength, ropeDefaultDuration);

        int maxColliders = 48;
        Collider[] hitColliders = new Collider[maxColliders];
        int hitCollidersCount = physicsScene.OverlapSphere(centerOfTentacles.position, 0.1f, hitColliders, toysLayer, QueryTriggerInteraction.UseGlobal);

        if(hitCollidersCount > 0) {
            canRelease = true;
            tentaclesCollider.SetToysColliding(hitColliders);
        }
        else {
            canRelease = false;
            timer.ShowCurrentTries();

            await DefaultTentacles(tentaclesCloseDuration);
            await MoveToDefault(moverMoveToDefaultDuration);
        }

        busy = false;
    }
    async public Task Release() {
        if(busy == true || canRelease == false || gameStarted == false) return;

        busy = true;

        DebugTimer openTentaclesTimer = CreateTimer();
        await OpenTentacles(tentaclesOpenDuration);

        DebugTimer defaultTentaclesTimer = CreateTimer();
        await DefaultTentacles(tentaclesCloseDuration);

        DebugTimer moveToDefaultTimer = CreateTimer();
        await MoveToDefault(moverMoveToDefaultDuration);

        canRelease = false;
        busy = false;
    }

    // ***
    //
    // корутина изменения длины веревки
    //
    // ***
    async private Task ChangeRopeLength(float toLength, float duration, bool stopWhenColliding = false) {
        float fromLength = rope.restLength;
        float passed = 0f;
        while(passed < duration) {

            // когда stopWhenColliding == true
            // мы ожидаем когда при изменении длины верёвки
            // мы будем соприкасаться с игрушками
            // когда соприкосновение происходит
            // мы увеличиваем длину верёвки ещё 500 миллисекунд
            // а затем останавливаем цикл (больше не изменяем длину верёвки)
            if(stopWhenColliding == true) {
                if(stopWhenCollidingLeftStarted == true) {
                    stopWhenCollidingLeft -= customFixedDeltaTime;

                    if(stopWhenCollidingLeft <= 0) {
                        stopWhenCollidingLeftStarted = false;
                        stopWhenCollidingLeft = 0;
                        toLength = rope.restLength;
                        break;
                    }
                }
                else {
                    int maxColliders = 1;
                    Collider[] hitColliders = new Collider[maxColliders];
                    int hitCollidersCount = physicsScene.OverlapSphere(centerOfTentacles.position, 0.1f, hitColliders, toysLayer, QueryTriggerInteraction.UseGlobal);

                    if(hitCollidersCount > 0) {
                        stopWhenCollidingLeftStarted = true;
                        stopWhenCollidingLeft = 0.5f;
                    }
                }
            }

            float length = Mathf.Lerp(fromLength, toLength, passed / duration);
            ropeCursor.ChangeLength(length);
            passed += customFixedDeltaTime;
            await CustomWaitForFixedUpdate();
        }
        ropeCursor.ChangeLength(toLength);
    }

    // ***
    //
    // корутины сжатия и расжатия клешни
    //
    // ***
    async private Task ToggleTentacles(float force, float min, float max, float duration) {
        HingeJoint joint1 = tentacle1.gameObject.GetComponent<HingeJoint>();
        HingeJoint joint2 = tentacle2.gameObject.GetComponent<HingeJoint>();
        HingeJoint joint3 = tentacle3.gameObject.GetComponent<HingeJoint>();

        JointMotor motor = joint1.motor;
        JointLimits limits = joint1.limits;

        motor.targetVelocity = 45f;
        motor.force = force;
        limits.min = min;
        limits.max = max;

        joint1.motor = joint2.motor = joint3.motor = motor;
        joint1.limits = joint2.limits = joint3.limits = limits;

        await CustomWaitForSeconds(duration);
    }
    async private Task OpenTentacles(float duration) {
        await ToggleTentacles(100f, -30f, 15f, duration);
    }
    async private Task CloseTentacles(float duration) {
        await ToggleTentacles(0f, -30f, 0f, duration);
    }
    async private Task DefaultTentacles(float duration) {
        await ToggleTentacles(100f, -30f, 0f, duration);
    }
}