// Copyright (c) 2018 ManusVR
namespace Assets.ManusVR.Scripts
{
    public class RegularWrist : Wrist {


        public override void Start()
        {
            base.Start();
            Rigidbody.isKinematic = true;
        }
    }
}
