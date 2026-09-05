using System;

namespace OM
{
    public interface IOMState<in T>
    {
        bool CanEnterState(T fromState) => true;
        void OnEnterState(T fromState);

        bool CanExitState(T toState) => true;
        void OnExitState(T toState);
        
        void OnUpdateState() { }
    }
    
    public class OMStateMachine<T> where T : IOMState<T>
    {
        public event Action<T, T> OnStateChanged;
        
        public T LastState { get; private set; }
        public T CurrentState { get; private set; }
        
        public OMStateMachine(T initState,Action<T,T> onStateChanged = null)
        {
            SetNewState(initState);
            if(onStateChanged != null) OnStateChanged += onStateChanged;
        }

        public bool SetNewState(T newState,bool force = false)
        {
            if(newState == null) return false;
            if (!force)
            {
                if (!newState.CanEnterState(fromState:CurrentState)) return false;
                if (CurrentState != null && !CurrentState.CanExitState(toState:newState)) return false;
            }
            CurrentState?.OnExitState(toState:newState);
            LastState = CurrentState;
            CurrentState = newState;
            CurrentState?.OnEnterState(fromState:LastState);
            OnStateChanged?.Invoke(CurrentState,LastState);
            return true;
        }

        public void UpdateStateMachine()
        {
            CurrentState?.OnUpdateState();
        }
        
        public void OnChange(Action<T,T> callback)
        {
            OnStateChanged += callback;
        }
    }
}