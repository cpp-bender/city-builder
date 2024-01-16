using UnityEngine;
using DG.Tweening;

public class Draggable : MonoBehaviour
{
    public float snapThreshold = 10F;
    public Transform pivotOffsetTransform;
    public Vector3 initRot;
    public BottomEffectObject bottomEffect;
    public ParticleSystem smokeEffect;

    [Header("DEBUG")]
    public bool selected;
    public bool isFirstSnapped;
    public bool isSnapped;
    public Selectable selectable;
    public bool canMakeMove;
    public bool isHitGameArea;
    public bool isHitDraggable;

    public float posY = 2F;
    private const float yAxisSpeed = 50F;
    private const float scaleSpeed = 2F;
    private GridManager gridManager;
    private Vector3 nextPos;
    private Vector3 offset;
    private bool canSnap;
    private BoxCollider boxCollider;
    private bool isSelectable = true;
    private bool isPopulated;

    private Sequence failSequence;
    private Tween moveDownTween;
    private Tween failMoveTween;
    private Tween failScaleTween;

    public GameObject placementParticle;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        canMakeMove = true;
        CreateBottomEffect();
        Select();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && selected && canMakeMove && isSelectable)
        {
            OnDrag();

            // Scaling();

            // YAxisMove();

            SwitchBottomEffectMaterial();
        }
        else if (selected)
        {
            Deselect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.GAMEAREA))
        {
            isHitGameArea = false;
            isHitDraggable = false;
        }

        if (other.CompareTag(Tags.DRAGGABLE))
        {
            isHitDraggable = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.GAMEAREA))
        {
            isHitGameArea = true;
            isHitDraggable = true;
        }

        if (other.CompareTag(Tags.DRAGGABLE))
        {
            isHitDraggable = true;
        }
    }

    public void SubscribeToSelectable() => selectable.draggables.Add(this);

    private void UnSubscribeToSelectable() => selectable.draggables.Remove(this);

    public void CloseSelectableProperty()
    {
        foreach (Transform childTransform in GetComponentInChildren<Transform>())
        {
            childTransform.gameObject.layer = 0;

            foreach (Transform grandChildTransform in childTransform.GetComponentInChildren<Transform>())
            {
                grandChildTransform.gameObject.layer = 0;
            }
        }
        isSelectable = false;
    }

    private void OnDrag()
    {
        moveDownTween.Kill();

        moveDownTween = null;

        var clampedVector = ClampToToGrid(GetMouseWorldPos() + offset, isFirstSnapped);

        var snapPos = gridManager.GetNearestCellPos(clampedVector, snapThreshold, out canSnap);

        if (!canSnap)
        {
            nextPos = clampedVector;

            var yPosition = isFirstSnapped ? posY : transform.position.y;

            transform.position = Vector3.Lerp(transform.position, new Vector3(nextPos.x, posY, nextPos.z), Time.deltaTime * 20f);
            //transform.position = new Vector3(nextPos.x, yPosition, nextPos.z);

            isSnapped = false;
        }
        else
        {
            //transform.position = new Vector3(snapPos.x, posY, snapPos.z);
            transform.position = Vector3.Lerp(transform.position, new Vector3(snapPos.x, posY, snapPos.z), Time.deltaTime * 20f);

            isFirstSnapped = true;

            isSnapped = true;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    private Vector3 ClampToToGrid(Vector3 vec, bool snappedAlready)
    {
        if (!snappedAlready)
        {
            return vec;
        }

        var gridSettings = LevelManager.Instance.currentGrid;

        int width = gridSettings.Width;
        int height = gridSettings.Height;

        float widthOffset = gridSettings.WidthOffset;
        float heightOffset = gridSettings.HeightOffset;

        Vector3 initialPos = gridSettings.InitialPos;

        float minPosX = initialPos.x;
        float minPosZ = initialPos.z;

        float maxPosX = width * widthOffset + initialPos.x;
        float maxPosZ = height * heightOffset + initialPos.z;

        maxPosX -= gridSettings.WidthOffset;
        maxPosZ -= gridSettings.HeightOffset;

        vec = new Vector3(Mathf.Clamp(vec.x, minPosX, maxPosX), 0f, Mathf.Clamp(vec.z, minPosZ, maxPosZ));

        return vec;
    }

    private void MakeMove()
    {
        if (moveDownTween != null)
        {
            return;
        }

        moveDownTween = transform.DOMoveY(-posY, .25f)
             .SetRelative(true).SetEase(Ease.InQuad)
             .OnComplete(delegate
             {
                 moveDownTween = null;
                 PlaySmokeEffect();
                 
                 if (!isPopulated)
                 {
                     GameManager.instance.CollectGem(transform.position,Mathf.RoundToInt(boxCollider.size.x * Random.Range(0.5f, 1.5f)));
                     isPopulated = true;
                 }
                 
             })
             .Play();

        gridManager.CheckGameOver();
    }

    private void CreateBottomEffect()
    {
        boxCollider = GetComponent<BoxCollider>();

        var localPosition = new Vector3(boxCollider.center.x, -1f, boxCollider.center.z);
        var localScale = new Vector3(boxCollider.size.x, 2f, boxCollider.size.z);

        bottomEffect = Instantiate(bottomEffect,transform);
        bottomEffect.TransformSetting(localPosition, localScale);
    }

    private void SwitchBottomEffectMaterial()
    {
        if(!isFirstSnapped)
        {
            bottomEffect.SwitchMaterial(false);
        }
        else 
        {
            bottomEffect.SwitchMaterial(!isHitDraggable);
        }
    }

    private void PlaySmokeEffect()
    {
        var smokeTemp = Instantiate(smokeEffect);
        var scaleRatio = ((boxCollider.size.x + boxCollider.size.z) / 2f) / 5f;

        var shape = smokeTemp.shape;
        shape.radius = scaleRatio;

        smokeTemp.transform.localScale = Vector3.one * (scaleRatio + 1f);
        smokeTemp.transform.position = pivotOffsetTransform.position;

        smokeTemp.Play();

        Destroy(smokeTemp.gameObject, 1f);
    }

    public void Select()
    {
        Transform cam = Camera.main.transform;

        Vector3 mobileOffset = cam.forward * 7f;

        offset = pivotOffsetTransform.position - GetMouseWorldPos() + mobileOffset;

        selected = true;
    }

    public void Deselect()
    {
        selected = false;

        if (isHitDraggable)
        {
            UndoMove();
        }
        else if (!isSnapped)
        {
            UndoMove();
        }
        else if (isHitGameArea)
        {
            UndoMove();
        }
        else if(isSelectable)
        {
            MakeMove();
        }
    }

    private void UndoMove()
    {
        canMakeMove = false;

        failSequence = DOTween.Sequence();

        failMoveTween = transform.DOMove(selectable.gameObject.transform.position, .5f);

        failScaleTween = transform.DOScale(Vector3.zero, .5f);

        UnSubscribeToSelectable();

        failSequence.Append(failMoveTween).Join(failScaleTween)
            .OnStart(delegate
            {
                //GetComponent<BoxCollider>().enabled = false;
            })
            .OnComplete(delegate
            {
                Destroy(gameObject);
            }).Play();
    }

    private void OnDestroy()
    {
        if(selectable != null)
            selectable.UpdateCount(1);
    }
}
