using UnityEngine;
using System.Collections;

public class LifeFollowHeroController : MonoBehaviour
{
    /// <summary>
    /// 3D target that this object will be positioned above.
    /// </summary>
    public UILabel life;
    public Transform target;
    /// <summary>
    /// Game camera to use.
    /// </summary>

    public Camera gameCamera;

    /// <summary>
    /// UI camera to use.
    /// </summary>

    public Camera uiCamera;

    /// <summary>
    /// Whether the children will be disabled when this object is no longer visible.
    /// </summary>

    public bool disableIfInvisible = true;

    Transform mTrans;
    bool mIsVisible = false;

    /// <summary>
    /// Cache the transform;
    /// </summary>

    void Awake() { mTrans = transform; }

    /// <summary>
    /// Find both the UI camera and the game camera so they can be used for the position calculations
    /// </summary>

    void Start()
    {


        /* if (target != null)
         {
             if (gameCamera == null) gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
             if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
             SetVisible(false);
         }
         else
         {
             Debug.LogError("Expected to have 'target' set to a valid transform", this);
             enabled = false;
         }*/
    }

    /// <summary>
    /// Enable or disable child objects.
    /// </summary>

    void SetVisible(bool val)
    {
        mIsVisible = val;

        for (int i = 0, imax = mTrans.childCount; i < imax; ++i)
        {
            NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
        }
    }

    /// <summary>
    /// Update the position of the HUD object every frame such that is position correctly over top of its real world object.
    /// </summary>

    void Update()
    {
        if (target == null)
        {
            if (GameObject.Find(name + "(Clone)"))
                target = GameObject.Find(name + "(Clone)").transform; //获取对应英雄的坐标
        }
        else if (target != null)
        {
            int x = target.gameObject.GetComponent<HeroController>().life;
            life.text = "" + x;
            Vector3 pos = gameCamera.WorldToViewportPoint(target.position);

            // Determine the visibility and the target alpha
            bool isVisible = (gameCamera.orthographic || pos.z > 0f) && (!disableIfInvisible || (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f));

            // Update the visibility flag
            if (mIsVisible != isVisible) SetVisible(isVisible);

            // If visible, update the position
            if (isVisible)
            {
                transform.position = uiCamera.ViewportToWorldPoint(pos);
                pos = mTrans.localPosition;
                pos.x = Mathf.FloorToInt(pos.x);
                pos.y = Mathf.FloorToInt(pos.y + 30);
                pos.z = 0f;
                mTrans.localPosition = pos;
            }
            OnUpdate(isVisible);
        }

    }

    /// <summary>
    /// Custom update function.
    /// </summary>
    //实现血条跟随对应英雄
    protected virtual void OnUpdate(bool isVisible) { }
}