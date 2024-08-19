using UnityEngine;

namespace TestAssignment._Project.Scripts.UI
{
    public class ShowOnMobile : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(Application.isMobilePlatform);
        }
    }
}
