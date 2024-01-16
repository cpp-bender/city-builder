using UnityEngine.Serialization;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Unity Utilities/Tween Params", fileName = "Tween Params")]
public class TweenParamsData : ScriptableObject
{
    private TweenParams tweenParams;

    [SerializeField, FormerlySerializedAs("Auto Kill")] bool autoKill = true;
    [SerializeField, FormerlySerializedAs("Delay Time")] public float delay = 0f;
    [SerializeField, FormerlySerializedAs("Ease Type")] public Ease ease = Ease.Linear;
    [SerializeField, FormerlySerializedAs("Loop Count")] public int loopCount = 1;
    [SerializeField, FormerlySerializedAs("Loop Type")] public LoopType loopType = LoopType.Yoyo;

    public TweenParams TweenParams
    {
        get
        {
            tweenParams = new TweenParams().SetAutoKill(autoKill).SetDelay(delay).SetEase(ease).SetLoops(loopCount, loopType);
            return tweenParams;
        }
    }
}