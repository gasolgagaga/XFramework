using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//通用缓存池模板
public abstract class CommonCacheModel<T,TEMP> 
{
    //最大缓存数量
    private int m_MaxCacheCount = 10;
    //最小缓存数量
    private int m_MinCacheCount = 40;
    //每次递增数量
    private int m_Unit = 1;
    //缓存警告数量
    private int m_WarningCount;
    //总的缓存数量
    private int m_AllCacheCount {
        get => m_AllCacheList.Count;
    }
    //可使用缓存数量
    private int m_EnableCacheCount {
        get => m_EnableCacheList.Count;
    }
    //使用中对象的数量
    private int m_UsedItemCount {
        get => m_UsedItemList.Count;
    }
    //所有的缓存对象列表
    private List<T> m_AllCacheList;
    //可使用的缓存列表
    private List<T> m_EnableCacheList;
    //使用中的对象列表
    private List<T> m_UsedItemList;
    //是否初始化
    private bool m_IsInit;
    //是否扩容
    private bool m_Expansion;
    //模板对象
    private TEMP m_Temp;
   

    protected CommonCacheModel(int _max, int _min,bool _expansion = true,int _unit = 1) {
        m_IsInit = false;
        m_Temp = default;
        m_MaxCacheCount = _max;
        m_MinCacheCount = _min;
        m_WarningCount = m_MaxCacheCount;
        m_AllCacheList = new List<T>(m_MinCacheCount);
        m_EnableCacheList = new List<T>(m_MinCacheCount);
        m_UsedItemList = new List<T>(m_MinCacheCount);
        m_Unit = _unit;
    }

    public void Initailization(TEMP _temp) {
        if (_temp == null || m_IsInit) {
            Debug.Log("Init error ! Temp is null or Init multipe times");
            return;
        }
        m_IsInit = true;
        m_Temp = _temp;
        //创建对象池
        for (int i = 0; i < m_MinCacheCount; i++) {
            T item = CreateItem(m_Temp);
            if (m_Temp == null) {
                break;
            }
            //重置对象
            ResetItem(item);
            m_AllCacheList.Add(item);
            m_EnableCacheList.Add(item);
        }
        //完成初始化之后调用的函数
        OnInit(m_Temp);
    }
    public void DisCard() {
        //释放所有可使用的缓存
        for (int i = 0; i < m_EnableCacheCount; i++) {
            DiscardItem(m_EnableCacheList[i]);
        }
        m_UsedItemList.Clear();
        m_EnableCacheList.Clear();
        m_Temp = default;
        OnDiscard();
    }

    public T PopItem() {
        //当前可用缓存不够时创建一个新的对象
        if (m_EnableCacheCount <= 0) {
            AddCache();
            //增加缓存超出上线警告
            if (m_AllCacheCount > m_WarningCount) {
                Debug.Log("Cache over max" + m_WarningCount);
                //如果为扩容池则扩容一半
                if (m_Expansion) {
                    m_MaxCacheCount += m_MaxCacheCount / 2;
                    m_WarningCount = m_MaxCacheCount;
                }
            }
        }
        if (m_EnableCacheCount > 0) {
            //取得最后一个对象
            T item = m_EnableCacheList[m_EnableCacheCount - 1];
            m_EnableCacheList.RemoveAt(m_EnableCacheCount - 1);
            m_UsedItemList.Add(item);
            return item;
        }

        //无数据则返回一个默认值
        return default;
    }

    public void PushBackItem(T _item) {
        if (_item == null) {
            return;
        }
        //如果使用中对象列表不包含回收对象则不做处理
        if (!m_UsedItemList.Contains(_item)) {
            return;
        }
        m_UsedItemList.Remove(_item);
        //如果为非扩容池则判断是否超出上限
        if (!m_Expansion) {
            if (m_AllCacheCount > m_MaxCacheCount) {
                //释放对象然后将其移除
                DiscardItem(_item);
                m_AllCacheList.Remove(_item);
                return;
            }
        }
        //放入缓存池之前先重置对象
        ResetItem(_item);       
        m_EnableCacheList.Add(_item);
    }

    //回收当前所有使用的对象
    public void PushBackAll() {
        for (int i = m_UsedItemCount - 1; i >= 0; i--) {
            PushBackItem(m_UsedItemList[i]);
        }
        m_UsedItemList.Clear();
    }

    private void AddCache() {
        for (int i = 0; i < m_Unit; i++) {
            T item = CreateItem(m_Temp);
            ResetItem(item);
            m_AllCacheList.Add(item);
            m_EnableCacheList.Add(item);
        }
    }

    //创建模板的函数
    protected abstract T CreateItem(TEMP _temp);
    //释放池的函数
    protected virtual void OnDiscard() { }
    //初始化池的函数
    protected abstract void OnInit(TEMP _temp);
    //重制对象的函数
    protected abstract void ResetItem(T _item);
    //释放对象的函数
    protected abstract void DiscardItem(T _item);
}
