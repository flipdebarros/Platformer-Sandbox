public abstract class BehaviourState {

	public virtual void OnEnter(Actor actor, Brain brain) { }
	public virtual void OnExit(Actor actor, Brain brain) { }

	public abstract void OnUpdate(Actor actor, Brain brain);
	public abstract BehaviourState OnHandleInput(Actor actor, Brain brain, InputType input);
	
}
