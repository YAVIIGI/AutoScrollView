using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace YA7GI
{
    public class AutoScrollContent : MonoBehaviour
    {
        [SerializeField]
        float margin = 10.0f;

        [Range(0.01f, 1.0f)]
        [SerializeField]
        float moveSecond = 1.0f;

        [SerializeField]
        AutoScrollEaseType easeType;

        // Start is called before the first frame update
        Vector2 currentPos = Vector2.zero;
        Vector2 nextPos = Vector2.zero;
        GameObject preSelectedObject;
        void Start()
        {
        }

        IEnumerator EMovePosition()
        {
            currentPos = transform.parent.GetComponent<RectTransform>().anchoredPosition;
            float sTime = Time.fixedTime;
            float rate = 1.0f - (Time.fixedTime - sTime);
            while (rate * moveSecond > 0.0f)
            {
                rate = 1.0f - (Time.fixedTime - sTime);
                float eType;
                switch (easeType)
                {
                    case AutoScrollEaseType.OutCubic:
                        eType = (1 - rate * rate * rate);
                        break;
                    case AutoScrollEaseType.OutSquare:
                        eType = (1 - rate * rate);
                        break;
                    case AutoScrollEaseType.Linear:
                    default:
                        eType = (1 - rate);
                        break;
                    
                }
                transform.parent.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(currentPos, nextPos, eType / moveSecond);
                yield return new WaitForFixedUpdate();
            }
            transform.parent.GetComponent<RectTransform>().anchoredPosition = nextPos;
        }

        // Update is called once per frame
        void Update()
        {
            GameObject selectedObject = GetContainedSelectedChild();
            if (selectedObject)
            {
                if (preSelectedObject != selectedObject)
                {
                    float diff = GetDistanceSelectedChild();
                    if (diff > 1.0f || diff < -1.0f)
                    {
                        nextPos = transform.parent.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, diff);
                        // transform.parent.GetComponent<RectTransform>().anchoredPosition = nextPos;
                        StartCoroutine(EMovePosition());
                    }
                }
            }
            preSelectedObject = selectedObject;
        }

        GameObject GetContainedSelectedChild()
        {
            foreach (GameObject g in GetChildren())
            {
                if (EventSystem.current.currentSelectedGameObject == g)
                {
                    return g;
                }
            }
            return null;
        }

        float GetDistanceSelectedChild()
        {
            GameObject go = GetContainedSelectedChild();
            if (go)
            {
                return IsContainedCompletely(go);
            }
            return 0.0f;
        }

        float IsContainedCompletely(GameObject target)
        {
            Vector2 mPos = transform.parent.GetComponent<RectTransform>().anchoredPosition;
            Vector2 pPos = transform.parent.parent.GetComponent<RectTransform>().anchoredPosition;
            Vector2 tPos = target.GetComponent<RectTransform>().anchoredPosition;
            Rect pRect = transform.parent.parent.GetComponent<RectTransform>().rect;
            Rect tRect = target.GetComponent<RectTransform>().rect;
            if (tRect.height + tRect.y + (-tPos.y) - mPos.y > pRect.height - margin)
            {
                return (tRect.height + tRect.y + (-tPos.y) - mPos.y) - pRect.height + margin;
            }
            else if (tRect.y + (-tPos.y) - mPos.y < margin)
            {
                return tRect.y + (-tPos.y) - mPos.y - margin;
            }
            return 0.0f;
        }

        List<GameObject> GetChildren()
        {
            var children = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform c = transform.GetChild(i);
                children.Add(c.gameObject);
            }
            return children;
        }
    }

}
