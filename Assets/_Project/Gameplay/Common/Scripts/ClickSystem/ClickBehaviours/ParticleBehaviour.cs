using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours.Base;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours
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