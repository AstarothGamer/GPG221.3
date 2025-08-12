using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Goap
{
    public class Action : MonoBehaviour
    {
        [SerializeField] public TMP_Text currentActionText;
        public string actionName;

        public WorldState worldState;
        public List<Prerequisite> prerequisits;
        public List<Effect> effects;

        public GameObject targetPosition;
        public bool isGuaranteed = false;
        public bool isMoving = false;
        public bool wasSuccesful = false;


        public virtual IEnumerator DoAction()
        {
            yield return null;
        }


        public virtual bool TryDoAction()
        {
            if (!isGuaranteed)
            {
                return false;
            }

            for (int i = 0; i < prerequisits.Count; i++)
            {
                bool has = false;
                for (int j = 0; j < worldState.receivedEffects.Count; j++)
                {
                    if (prerequisits[i].name == worldState.receivedEffects[j].name)
                    {
                        has = true;
                        break;
                    }
                }

                if (!has)
                {
                    currentActionText.text = "I cannot do this " + actionName + ", because I miss " + prerequisits[i].name + ".";
                    return false;
                }
            }

            return true;
        }

        public virtual void ApplyEffects()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                bool alreadyHas = false;

                for (int j = 0; j < worldState.receivedEffects.Count; j++)
                {
                    if (effects[i].name == worldState.receivedEffects[j].name)
                    {
                        alreadyHas = true;
                        break;
                    }
                }

                if (!alreadyHas)
                {
                    worldState.receivedEffects.Add(effects[i]);
                }
            }
        }
    }
}
