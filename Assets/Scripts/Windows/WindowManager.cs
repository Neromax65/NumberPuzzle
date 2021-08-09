using UnityEngine;

namespace Windows
{
    public class WindowManager : MonoBehaviour
    {
        public WindowItem MainMenuWindow => menusItems[0];
        public WindowItem SelectionWindow => menusItems[1];
        public WindowItem VictoryWindow => menusItems[2];
    
        [SerializeField] private WindowItem[] menusItems;

        private void Start()
        {
            MainMenuWindow.Show();
        }
    }
}
