using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace W25SpaceShipDemo
{
    [DataContract]
    public class ShipBehaviour : Behavior
    {
        [RequiredComponent] // At runtime, Wave will set this property for us
        public Transform3D Transform;

        [DataMember] // Editable from editor
        public float Speed { get; set; }

        private float currentSpeed;

        protected override void Initialize()
        {
            base.Initialize();

            this.currentSpeed = this.Speed;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var rotation = Vector3.Zero;
            var input = WaveServices.Input.KeyboardState;

            if(input.W == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y -= (float)gameTime.TotalSeconds;
            }

            if (input.S == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y += (float)gameTime.TotalSeconds;
            }

            if (input.A == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X += (float)gameTime.TotalSeconds;
            }

            if (input.D == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X -= (float)gameTime.TotalSeconds;
            }

            this.Transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);

            var localPosition = this.Transform.LocalPosition;

            //localPosition.Z -= this.currentSpeed * (float)gameTime.TotalSeconds;
            //this.Transform.LocalPosition = localPosition;

            this.Transform.LocalPosition += (float)gameTime.TotalSeconds * this.currentSpeed * this.Transform.WorldTransform.Forward;
        }
    }
}
