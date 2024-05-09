    using DefaultNamespace;
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class AHealthDisplayer : MonoBehaviour, IHealthDisplayable
    {
        public Image healthFillImage;


        public abstract void UpdateHealth(int maxHealth, int curHealth);
    }
