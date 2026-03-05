using UnityEngine;
using System.Collections;

public class SimpleTeleport : MonoBehaviour
{
    public Transform 目标位置;
    public LoadingController 加载控制器;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" || other.gameObject.tag == "Player")
        {
            StartCoroutine(传送流程(other.gameObject));
        }
    }

    System.Collections.IEnumerator 传送流程(GameObject 玩家)
    {
        PlayerController 控制脚本 = 玩家.GetComponent<PlayerController>();
        if (控制脚本 != null) 控制脚本.canMove = false;

        if (加载控制器 == null)
        {
            Debug.LogError("传送错误：加载控制器未连接！");
            if (控制脚本 != null) 控制脚本.canMove = true;
            yield break;
        }

        // 显示加载画面
        加载控制器.ShowLoading();

        // 等待淡入完成
        yield return new WaitForSeconds(0.5f);

        // 传送玩家
        if (目标位置 != null)
        {
            玩家.transform.position = 目标位置.position;
        }

        // 等待随机时间（让加载画面显示一段时间）
        // 不需要等待固定5秒，LoadingController会自动控制显示时间
        yield return new WaitForSeconds(Random.Range(加载控制器.minShowTime, 加载控制器.maxShowTime));

        // 注意：如果LoadingController有自己的淡出逻辑，这里不需要重新启用移动
        // 可以在LoadingController的HideWithFade完成后启用移动
        if (控制脚本 != null) 控制脚本.canMove = true;
    }
}