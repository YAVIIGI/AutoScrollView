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
                    Vector2 diff = GetDistanceSelectedChild();
                    if (diff.magnitude > 1.0f || diff.magnitude < -1.0f)
                    {
                        nextPos = transform.parent.GetComponent<RectTransform>().anchoredPosition + diff;
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

        Vector2 GetDistanceSelectedChild()
        {
            GameObject go = GetContainedSelectedChild();
            if (go)
            {
                return CheckContainedCompletely(go);
            }
            return Vector2.zero;
        }

        Vector2 CheckContainedCompletely(GameObject target)
        {
            Vector2 cPos = transform.parent.GetComponent<RectTransform>().anchoredPosition;         // content
            Vector2 vPos = transform.parent.parent.GetComponent<RectTransform>().anchoredPosition;  // viewport
            Vector2 sPos = target.GetComponent<RectTransform>().anchoredPosition;                   // selected child
            Rect vRect = transform.parent.parent.GetComponent<RectTransform>().rect;
            Rect sRect = target.GetComponent<RectTransform>().rect;

            float x = 0, y = 0;

            if (sPos.x + sRect.x + sRect.width + cPos.x > vRect.width - margin)
            {
                x = -(sPos.x + sRect.x + sRect.width + cPos.x - (vRect.width - margin));
            }
            else if (sPos.x + sRect.x + cPos.x < margin)
            {
                x = -(sPos.x + sRect.x + cPos.x - (margin));
            }

            if (sRect.height + sRect.y + (-sPos.y) - cPos.y > vRect.height - margin)
            {
                y = (sRect.height + sRect.y + (-sPos.y) - cPos.y) - vRect.height + margin;
            }
            else if (sRect.y + (-sPos.y) - cPos.y < margin)
            {
                y = sRect.y + (-sPos.y) - cPos.y - margin;
            }

            return new Vector2(x, y);
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
