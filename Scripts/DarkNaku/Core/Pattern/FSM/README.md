# 개요
MonoBehaviour 베이스에 간단한 FSM(Finite State Machine) 프레임워크입니다.

## 개발 예정 기능

* 상태별로 전환 가능한 다음 상태 지정 기능
* ~~다음 상태에 매개변수 전달 기능~~
* Execute 함수를 명시적인 호출하는 대신 Update와 FixedUpdate 예약 함수 오버라이드

## 클래스

**FSMBehaviour\<S, M>** 는 머신 클래스이고 S는 enum 타입 M은 FSMBehaviour를 상속받는 클래스의 타입입니다.

```
public enum STATE { IDLE, MOVE, LOCK }
public class MachineController : FSMBehaviour<STATE, MachineController>
```

**FSMState\<S, M>** 상태 클래스이며 S와 M의 타입은 머신 클래스와 동일한 타입을 받아야 합니다.

```
public class IdleState : FSMState<STATE, MachineController>
```

## 속성

### FSMBehaviour\<S, M>

**public StateChangeEvent OnTransition**

상태의 변화에 대해서 알림을 받을 수 있는 이벤트 속성입니다. 이전 상태와 다음 상태를 변수로 알려줍니다.

**public S State**

현재 상태를 확인할 수 있는 속성값 입니다.

### FSMState\<S, M>

**protected M Machine**

머신을 참조하고 있는 속성 입니다.

**public abstract S State**

상태를 정의하는 속성이며, 상태 클래스를 상속받는 경우 반드시 구현해 줘야 합니다.

**protected object Param**

다음 상태에 전달할 매개변수를 일시적으로 전달할 용도의 변수이며 다음 상태의 OnEnter 이벤트의 파라미터로 전달됩니다.

## 함수

### FSMBehaviour\<S, M>

**protected void AddStates(params FSMState\<S, M>[] states)**

머신에 상태를 추가하기 위한 함수입니다.

**protected void RemoveState(S state)**

머신에서 상태를 삭제하기 위한 함수입니다.

**protected void Execute()**

머신을 주기적으로 갱신하기 위한 함수 입니다. 이 함수를 호출해야 머신이 동작합니다. 보통은 Update 또는 FixedUpdate에서 호출해 주면 됩니다.

### FSMState\<S, M>

**public virtual void OnInitialize()**

상태 객체가 머신에 등록될 때 한번 호출 됩니다.

**public virtual void OnEnter(object param)**

상태에 진입할 때 한번 호출 됩니다. param의로 이전 상태에서 저장한 Param 변수의 값이 전달됩니다.

**public virtual void OnLeave()**

상태에서 빠저나올 때 한번 호출 됩니다.

**public virtual S NextState()**

다음 상태로 전이할 조건을 판단하여 다음 상태 값을 반환하는 함수 입니다. 조건에 해당하지 않으면 현재 상태를 반환하면 됩니다. 오버라이드 하지 않는 경우 기본적으로 현재 상태를 반환하도록 되어 있습니다.

**public virtual void Update()**

매 실행시마다 해야하는 행동을 정의하는 함수입니다.

## 이벤트 순서에 대한 추가내용

매 프레임에 Execute를 호출 한다는 가정하에 설명하면 상태 전환시 NextState가 호출 된 프레임에 다음 상태의 OnEnter와 이전 상태의 OnLeave가 호출되고 다음 상태의 Update가 호출 됩니다. 이전 상태의 Update는 전환이 일어나는 프레임에는 호출되지 않습니다. 상태 판단과 행동을 한번에 하는경우 무한룹에 빠지는 문제가 발생 할 수 있어서 이와 같이 설계하였습니다.
