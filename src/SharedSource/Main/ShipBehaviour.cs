using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
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

        private VirtualScreenManager vm;

        private float centerX, centerY;

        private Entity lbScoreEntity;

        [DataMember]
        [RenderPropertyAsEntity()]
        public string LabelScoreEntityPath { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            this.currentSpeed = this.Speed;

            this.vm = this.Owner.Scene.VirtualScreenManager;

            centerX = this.vm.RightEdge - this.vm.LeftEdge;
            centerY = this.vm.BottomEdge - this.vm.TopEdge;

            this.lbScoreEntity = this.EntityManager.Find(this.LabelScoreEntityPath);
        }

        protected override void Update(TimeSpan gameTime)
        {
            var rotation = Vector3.Zero;
            var input = WaveServices.Input.KeyboardState;

#if WINDOWS
            if (input.W == WaveEngine.Common.Input.ButtonState.Pressed)
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
#endif

            TouchPanelState state = WaveServices.Input.TouchPanelState;

            foreach (var touch in state)
            {
                Vector2 touchPosition = touch.Position;

                var touchFactorX = touchPosition.X > this.centerX ? -0.001f : 0.001f;
                var touchFactorY = touchPosition.Y > this.centerY ? -0.001f : 0.001f;

                var deltaFromCenterX = Math.Abs(touchPosition.X - this.centerX);
                var deltaFromCenterY = Math.Abs(touchPosition.Y - this.centerY);

                rotation.X += (float)gameTime.TotalSeconds * deltaFromCenterX * touchFactorX;
                rotation.Y += (float)gameTime.TotalSeconds * deltaFromCenterY * touchFactorY;
            }

            this.Transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);

            var localPosition = this.Transform.LocalPosition;

            //localPosition.Z -= this.currentSpeed * (float)gameTime.TotalSeconds;
            //this.Transform.LocalPosition = localPosition;

            this.Transform.LocalPosition += (float)gameTime.TotalSeconds * this.currentSpeed * this.Transform.WorldTransform.Forward;

            // Update score
            Game.score++;
            this.lbScoreEntity.FindComponent<TextComponent>().Text = Game.score.ToString();
        }

        public void Reset()
        {
            Debug.WriteLine($"ShipBehavior - RESET");
            this.Owner.IsVisible = true;
            this.Transform.Position = Vector3.Zero;
            this.Transform.Rotation = Vector3.Zero;
            this.currentSpeed = this.Speed;
        }

        public void GameOver()
        {
            Debug.WriteLine($"ShipBehavior - GAME OVER");
            this.Owner.IsVisible = false;
            this.currentSpeed = 0;
        }
    }
}
