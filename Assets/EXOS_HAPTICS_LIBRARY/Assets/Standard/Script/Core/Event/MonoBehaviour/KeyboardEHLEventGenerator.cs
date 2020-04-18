using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using UnityEngine;

namespace exiii.Unity
{
    public class KeyboardEHLEventGenerator : EHLEventGeneratorBase
    {
        #region Inspector

        [Header("KeyboardEventGenerator")]
        [SerializeField]
        private KeyCode UseButton;

        [SerializeField]
        private KeyCode GrabButton;

        #endregion

        private void Start()
        {
            this.UpdateAsObservable().Where(_ => Input.GetKeyDown(UseButton)).Subscribe(UseEventGenerator.Start);
            this.UpdateAsObservable().Where(_ => Input.GetKey(UseButton)).Subscribe(UseEventGenerator.Stay);
            this.UpdateAsObservable().Where(_ => Input.GetKeyUp(UseButton)).Subscribe(UseEventGenerator.End);

            this.UpdateAsObservable().Where(_ => Input.GetKeyDown(GrabButton)).Subscribe(GrabEventGenerator.Start);
            this.UpdateAsObservable().Where(_ => Input.GetKey(GrabButton)).Subscribe(GrabEventGenerator.Stay);
            this.UpdateAsObservable().Where(_ => Input.GetKeyUp(GrabButton)).Subscribe(GrabEventGenerator.End);
        }
    }
}