SUPER HOT 2D

�̸� : ������2D
���� : �������� 2D ž�ٿ��� �ű� ����. 
�̽� ��� :
1. �Է°� �ð� ������
2. ���� ������ ����. 
3. �þ�(������ �Ȱ�)
4. ���� �� ����
5. �� ĳ���Ϳ� AI


�Է°� �ð� ������
Update�Լ����� �Էµ� ��� ���콺 ��ġ�� ���� TimeScaleManager���� �Էµ� ������ ����. �÷��̾� ĳ���Ϳ��� �Էµ� Ű (��Ŭ��, ��Ŭ��, E) ����.

InputHandlertsm : TimeScaleManager
+player : GameObject
+canInput : bool
-preMousePos : Vector3-Update()
-TimeScaleInput(inputMousePos, inputDir)
-PlayerMoveInput(inputMousePos, inputDir)
-CommandToRotate(angle) 
 Update ���ο��� �Էµ� ���� Vector2�� ���� direction�� ���콺�� ��ġ�� PlayerMoveInput�� TimeScaleInput�� �����մϴ�. ��Ŭ��, ��Ŭ��, EŰ�� ���ȴٸ� player���� �޽����� �����ϴ�. 
 TimeScaleInput�� preMousePos�� �Ű����� inputMousePos�� ���Ͽ� ���콺�� ������ ������ ĳ���� �̵� Ű ������ �Ǵ��Ͽ� �Էµ� ������ enum INPUTTYPE�� �̿��Ͽ� �����մϴ�.
 PlayerMoveInput�� Command�� �Էµ� ����� ���콺�� ���� ȸ���� �����մϴ�.



 TimeScaleManager-scaler : readonly float
-canScale : bool
-target : float-ControlTimeScale() : IEnumerator
+SetInputType(INPUTTYPE input)
+SetTimeScaleImmediately(float to)
+SetScaleSwitch(bool isTurnOn)
+FixTimeScale(float scale, float duration)
-TurnOnTimer(float duration) : IEnumerator
SetInputType�� ���ؼ� �Է������� �޽��ϴ�. �Է��� ���� �� 0.05, ���콺 �̵� �� 0.25, Ű���� �Է� �� 1�� target�� �����մϴ�. 
ControlTimeScale �ڷ�ƾ�� �̿��Ͽ� ���� TimeScale�� target�� ����ϴ�. TimeScale�� ������ �� ������ �� ���� 2�� ������ �����մϴ�. Realtime 0.025�ʸ��� 0.03�� lerp �մϴ�.
FixTimeScale�� TimeScale�� scale������ �����ð����� duration��ŭ �����մϴ�.

InputHandler�� player���� MoveTo �޽����� ������ MoveUnitCommand���� �޾� ������ �����մϴ�. MoveUnitCommand�� Update������ Execute�� ȣ���ϸ� MoveHandler���� MoveToDir �޼��带 ȣ���ϰ� MoveHandler�� ĳ������ ���°� ������ �� ������ rigidbody2d�� velocity�� �����Ͽ� ĳ���͸� �����Դϴ�.
MoveUnitCommand-space : Space
-dir : Vector2
-mh : MoveHandler-Awake()
+MoveUnitCommand(dir, space = Space.World)
-Update()
-Execute()
+MoveTo(direction)MoveHandler-rgbd : Rigidbody2D
-sw : bool
-speed : float-Awake()
-StateObserver(state)
+MoveToWorldDir(dir)
+MoveToSelfDir(dir)
RotateUnitCommand+LookAt(angle)
ĳ������ ȸ���� TimeScale�� ������� �۵��մϴ�.




