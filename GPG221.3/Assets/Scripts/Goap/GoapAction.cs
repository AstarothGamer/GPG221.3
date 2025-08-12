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


        public virtual void DoAction()
        {
            StartCoroutine(SmoothMoving(targetPosition.transform.position));
        }

        private IEnumerator SmoothMoving(Vector3 direction)
        {
            isMoving = true;

            Vector3 startPos = transform.position;
            Vector3 endPos = direction;
            float duration = 4f;
            float time = 0f;

            currentActionText.text = "Going to " + targetPosition.name + ".";
            while (time < duration)
            {
                float speed = Vector3.Distance(startPos, endPos) / duration;
                transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);

                time += Time.deltaTime;
                yield return null;
            }
            currentActionText.text = "Came to " + targetPosition.name + ".";

            yield return new WaitForSeconds(1f); ;

            transform.position = endPos;
            isMoving = false;

            currentActionText.text = "Starting do " + actionName + ".";
            yield return new WaitForSeconds(1f);

            if (TryDoAction())
            {
                ApplyEffects();
                wasSuccesful = true;
            }
            else
            {
                wasSuccesful = false;
            }
            yield return new WaitForSeconds(1f); 
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
