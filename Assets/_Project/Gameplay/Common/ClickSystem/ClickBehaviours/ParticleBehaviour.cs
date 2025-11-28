using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "Click Behaviours/Particle Behaviour")]
    public class ParticleBehaviour : ClickBehaviour
    {
        [SerializeField] private ParticleSystem clickParticle;
    
        public override void Execute(GameObject clickedObject)
        {
            Instantiate(clickParticle, clickedObject.transform.position, Quaternion.identity);
        }
    }
}