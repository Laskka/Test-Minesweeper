using UnityEngine;

namespace App.Presentation
{
    public class ViewRoot : MonoBehaviour
    {
        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}