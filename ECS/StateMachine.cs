using System;

namespace Navy.ECS
{
    public class StateMachine : Component, IUpdateableComponent
    {
        public State CurrentState { get; private set; }

        public void Transition<T>() where T : State
        {
            CurrentState?.OnExit();

            CurrentState = (T)Activator.CreateInstance(typeof(T));
            CurrentState.FSM = this;
            CurrentState.OnEnter();
        }

        public void Update(GameTime gameTime)
        {
            CurrentState.Update(gameTime);
        }
    }

    public abstract class State
    {
        public StateMachine FSM { get; set; }
        public abstract void Update(GameTime gameTime);
        public abstract void OnEnter();
        public abstract void OnExit();
    }
}
