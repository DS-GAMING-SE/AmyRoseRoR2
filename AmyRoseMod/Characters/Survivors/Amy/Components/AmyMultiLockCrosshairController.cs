using UnityEngine;
using RoR2;
using RoR2.UI;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;

namespace AmyRoseMod.Characters.Survivors.Amy.Components
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(HudElement))]
    public class AmyMultiLockCrosshairController : MonoBehaviour
    {
        public HudElement hudElement;
        public EntityStateMachine stateMachine;
        public MultiLockTargeting multiLockState;

        public GameObject[] multiLockHearts;
        public int numHearts;
        public int numHeartsFilled;

        public static GameObject multiLockHeartPrefab;
        public const float circleRadius = 65;
        private void Awake()
        {
            hudElement = GetComponent<HudElement>();
        }

        public void Start()
        {
            if (hudElement && hudElement.targetBodyObject)
            {
                stateMachine = EntityStateMachine.FindByCustomName(hudElement.targetBodyObject, "Weapon2");
                if (stateMachine && stateMachine.state is MultiLockTargeting)
                {
                    multiLockState = stateMachine.state as MultiLockTargeting;
                    numHearts = multiLockState.maxTargets;
                    multiLockHearts = new GameObject[numHearts];
                    numHeartsFilled = numHearts;
                    float angle = 360f / numHearts;
                    float angleRadian = angle * 0.0174f;
                    for (int i = 0; i < multiLockHearts.Length; i++)
                    {
                        multiLockHearts[i] = GameObject.Instantiate(multiLockHeartPrefab, gameObject.transform);
                        multiLockHearts[i].transform.localPosition = new Vector3(Mathf.Sin(angleRadian * i) * circleRadius, Mathf.Cos(angleRadian * i) * circleRadius, 0);
                        multiLockHearts[i].transform.localRotation = Quaternion.AngleAxis(angle * -i, Vector3.forward);
                    }
                    multiLockState.OnTargetsChanged += UpdateHearts;
                    UpdateHearts(multiLockState.targets.Count);
                }
            }
        }

        public void UpdateHearts(int count)
        {
            int desiredFilledHearts = numHearts - count;
            if (numHeartsFilled == desiredFilledHearts) { return; }
            if (numHeartsFilled > desiredFilledHearts)
            {
                for (int i = numHeartsFilled - 1; i >= 0; i--)
                {
                    if (numHeartsFilled > desiredFilledHearts)
                    {
                        multiLockHearts[i].transform.Find("MultiLockHeartFill").gameObject.SetActive(false); // don't Find() all the damn time
                        numHeartsFilled--;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                for (int i = numHeartsFilled; i < numHearts; i++)
                {
                    if (numHeartsFilled < desiredFilledHearts)
                    {
                        multiLockHearts[i].transform.Find("MultiLockHeartFill").gameObject.SetActive(true); // don't Find() all the damn time
                        numHeartsFilled++;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}