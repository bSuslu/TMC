using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "SO/Click Behaviours/Particle Behaviour")]
    public class ParticleBehaviour : ClickBehaviour
    {
        [SerializeField] private ParticleSystem _clickParticle;
    
        public override void Execute(ItemEntity clickedObject)
        {
            Instantiate(_clickParticle, clickedObject.transform.position, Quaternion.identity);
        }
    }
}