using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

namespace W25SpaceShipDemo
{
    [DataContract]
    public class AsteroidManager : Behavior
    {
        private bool isSpawned;

        [DataMember]
        public int NumberOfAsteroids { get; set; }

        private List<Entity> asteroids;

        private int asteroidIndex;

        [DataMember]
        public float AsteroidInterval { get; set; }

        [DataMember]
        public float SpawnDistance;

        [DataMember]
        public int AsteroidSpread { get; set; }

        [DataMember]
        [RenderPropertyAsEntity]
        public string ShipPath { get; set; }

        private float remainingAsteroidTime;

        private Entity shipEntity;

        private float remainingGameOverTime;

        private bool isGameOver;

        protected override void Initialize()
        {
            base.Initialize();

            if(!string.IsNullOrEmpty(this.ShipPath))
            {
                this.shipEntity = this.EntityManager.Find(this.ShipPath);
            }            
        }

        protected override void Update(TimeSpan gameTime)
        {
            if(!this.isSpawned)
            {
                // First frame
                this.CreateAsteroids();
                this.isSpawned = true;

                // Subscribe to collision event
                var sphereCollider = this.shipEntity.FindComponent<SphereCollider3D>();
                sphereCollider.PhysicBody.BeginCollision += PhysicBody_BeginCollision;

                return;
            }

            if (this.shipEntity == null && this.NumberOfAsteroids == 0)
            {
                return;
            }

            if (this.isGameOver)
            {
                this.remainingGameOverTime -= (float)gameTime.TotalSeconds;

                if (this.remainingGameOverTime < 0)
                {
                    this.Reset();
                }                
            }
            else
            {
                this.remainingAsteroidTime -= (float)gameTime.TotalSeconds;

                if (this.remainingAsteroidTime < 0)
                {
                    this.ShowAsteroid();
                    this.remainingAsteroidTime += this.AsteroidInterval;
                }
            }
        }

        private void PhysicBody_BeginCollision(object sender, CollisionInfo3D e)
        {
            if (!this.isGameOver)
            {
                this.GameOver();
                Debug.WriteLine($"Ship collided with {e.OtherBody.Owner.Name}");
            }
        }

        private void Reset()
        {
            Debug.WriteLine($"AsteroidManager - RESET");

            foreach (var asteroid in this.asteroids)
            {
                asteroid.IsVisible = false;
            }

            this.shipEntity.FindComponent<ShipBehaviour>().Reset();

            this.isGameOver = false;
        }

        private void GameOver()
        {
            Debug.WriteLine($"AsteroidManager - GAME OVER");
            this.isGameOver = true;

            this.shipEntity.FindComponent<ShipBehaviour>().GameOver();

            this.remainingGameOverTime = 3;
        }

        private void ShowAsteroid()
        {
            var asteroid = this.asteroids[this.asteroidIndex];
            asteroid.IsVisible = true;

            var shipTransform = this.shipEntity.FindComponent<Transform3D>();

            var position = shipTransform.Position + (shipTransform.WorldTransform.Forward * this.SpawnDistance);
            position.X += WaveServices.Random.Next(-this.AsteroidSpread, this.AsteroidSpread) * 0.01f;
            position.Y += WaveServices.Random.Next(-this.AsteroidSpread, this.AsteroidSpread) * 0.01f;

            var transform = asteroid.FindComponent<Transform3D>();
            transform.Position = position;
            transform.Scale = new WaveEngine.Common.Math.Vector3(WaveServices.Random.Next(1, 3));

            var spinner = asteroid.FindComponent<Spinner>();
            spinner.IncreaseX = WaveServices.Random.Next(-100, 100) * 0.01f;
            spinner.IncreaseY = WaveServices.Random.Next(-100, 100) * 0.01f;
            spinner.IncreaseZ = WaveServices.Random.Next(-100, 100) * 0.01f;

            var spawnBehavior = asteroid.FindComponent<SpawnBehavior>();
            spawnBehavior.Spawn();

            this.asteroidIndex = (this.asteroidIndex + 1) % this.NumberOfAsteroids;
        }

        private void CreateAsteroids()
        {
            this.asteroids = new List<Entity>();

            for (int i = 0; i < this.NumberOfAsteroids; i++)
            {
                var asteroid = this.CreateAsteroid(i);
                this.asteroids.Add(asteroid);

                this.EntityManager.Add(asteroid);
            }

            this.asteroidIndex = 0;            
        }

        private Entity CreateAsteroid(int i)
        {
            string model;

            switch(i % this.NumberOfAsteroids)
            {
                case 0:
                    model = WaveContent.Assets.Models.asteroid_1_0_fbx;
                    break;
                case 1:
                    model = WaveContent.Assets.Models.asteroid_2_0_fbx;
                    break;
                case 2:
                    model = WaveContent.Assets.Models.asteroid_3_0_fbx;
                    break;
                default:
                    model = WaveContent.Assets.Models.asteroid_3_0_fbx;
                    break;
            }

            var asteroid = new Entity("asteroidCreatedProgrammatically" + i)
                .AddComponent(new Transform3D())
                .AddComponent(new MaterialComponent() { MaterialPath = WaveContent.Assets.Materials.asteroidMat })
                .AddComponent(new FileMesh() { ModelPath = model })
                .AddComponent(new MeshRenderer())
                .AddComponent(new Spinner())
                .AddComponent(new SpawnBehavior())
                .AddComponent(new SphereCollider3D())
                .AddComponent(new StaticBody3D());

            asteroid.IsVisible = false;

            return asteroid;
        }
    }
}
