
using System.Collections;

using System.Collections.Generic;
using System;

/// <summary>
/// 事件派发器，针对自己本身的事件监听基类
/// </summary>
public class EventDispatcher {

	public delegate void EventHandler(object sender);
	//public EventHandler Event;
	private Dictionary<string,EventHandler> events = new Dictionary<string, EventHandler>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eventHandle"></param>
	public void AddEventListener(string id,EventHandler eventHandle)
	{
		if(events.ContainsKey(id))
		{
			events[id] += eventHandle;
		}
		else
		{
			events.Add(id ,eventHandle);
		}
	}

    /// <summary>
    /// 检查是否含有某个事件的监听
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
	public bool HasEventListener(string id)
	{
		if(events.ContainsKey(id))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eventHandle"></param>
	public void RemoveEventListener(string id,EventHandler eventHandle)
	{
		if(events.ContainsKey(id))
		{
			events[id]-=eventHandle;
			if(events[id]==null)
			{
				events.Remove(id);
			}
		}
	}

    /// <summary>
    /// 派发事件监听
    /// </summary>
    /// <param name="id"></param>
	public void Dispatcher(string id)
	{
		GC.Collect(); 
		if(events.ContainsKey(id))
		{
			events[id](this);
		}
	}


}
