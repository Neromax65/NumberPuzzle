using UnityEngine;

namespace Windows
{
    /// <summary>
    /// Абстрактный класс для быстрого доступа к окнам
    /// </summary>
    public abstract class WindowItem : MonoBehaviour
    {
        
        /// <summary>
        /// Показать окно
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Скрыть окно
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}