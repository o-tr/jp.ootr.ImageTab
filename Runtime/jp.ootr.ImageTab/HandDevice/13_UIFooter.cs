using jp.ootr.common;
using UnityEngine;

namespace jp.ootr.ImageTab.HandDevice
{
    public class UIFooter : UIDeviceList
    {
        [SerializeField] protected Transform uIFooter;

        public override void InitController()
        {
            base.InitController();
            UpdateFooter();
        }

        public override void OnDirectionChanged()
        {
            base.OnDirectionChanged();
            UpdateFooter();
        }

        public virtual void UpdateFooter()
        {
            uIFooter.ToFillChildrenHorizontal(0, 2);
        }
    }
}