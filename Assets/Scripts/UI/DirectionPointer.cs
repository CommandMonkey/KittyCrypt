using UnityEngine;

public class DirectionPointer : MonoBehaviour
{
    [SerializeField] float arrowScreenOfset = 1f;
    public Transform target;
    public Transform arrow;
    void Start()
    {
        arrow.localPosition = new Vector3(0, Screen.height / 2 - arrowScreenOfset);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target.transform.position);
        bool onScreen = viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
        if (!onScreen)
        {
            arrow.gameObject.SetActive(true);

            Vector3 thisPos = Camera.main.ScreenToWorldPoint(transform.position);

            Vector2 direction = target.transform.position - thisPos;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90f;
            // Create a quaternion rotation based on the angle
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = rotation;
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
    }
}
