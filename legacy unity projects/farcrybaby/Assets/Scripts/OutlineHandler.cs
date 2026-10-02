using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class OutlineHandler : MonoBehaviour
    {
        public static OutlineHandler inst;

        public Camera cam;
        public float width = 0.1f;
        public Color color;

        private List<OutlineCouple> outlines;

        private void Start()
        {
            inst = this;
            outlines = new List<OutlineCouple>();
        }

        private void LateUpdate()
        {
            if (outlines.Count == 0) return;
            foreach (OutlineCouple c in outlines)
            {
                if (c.original != null && c.original.activeSelf)
                {
                    Vector3 scale = c.original.transform.localScale + Vector3.one * width;
                    c.outline.transform.localScale = scale;
                    c.outline.transform.eulerAngles = c.original.transform.eulerAngles;
                    c.outline.transform.position = c.original.transform.position + cam.transform.forward * width * 2f;
                }
                else
                {
                    c.outline.SetActive(false);
                }
            }
            outlines[outlines.Count - 1].outline.GetComponent<Renderer>().sharedMaterial.SetColor("_MainColor", color);
        }

        public void Show(Actor actor)
        {
            foreach (Renderer renderer in actor.GetRenderers())
            {
                outlines.Add(new OutlineCouple(renderer.gameObject, Pools.Get(PoolType.OutlineCube)));
            }
        }

        public void Clear()
        {
            foreach (OutlineCouple c in outlines)
            {
                c.outline.SetActive(false);
            }
            outlines.Clear();
        }

        public class OutlineCouple
        {
            public GameObject original;
            public GameObject outline;

            public OutlineCouple(GameObject original, GameObject outline)
            {
                this.original = original;
                this.outline = outline;
            }
        }
    }
}
