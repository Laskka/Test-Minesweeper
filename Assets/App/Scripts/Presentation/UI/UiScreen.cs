using UnityEngine;

namespace App.Presentation.UI
{
    public abstract class UiScreen : MonoBehaviour
    {
        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}