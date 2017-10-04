using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorPinsGroup
{
    public enum GroupState
    {
        OFF,
        ACTIVE,
        COMBINED,
        REMOVE
    }

    public int index;
    public bool isAnalizing;
    public GroupState currentState = GroupState.OFF;
    public List<Circumference> members = new List<Circumference>();

    public bool isActive
    {
        get { return currentState == GroupState.ACTIVE; }
    }

    public void CombineWith(int value)
    {
        SetState(GroupState.COMBINED);
    }

    public ColorPinsGroup(int id, Circumference member = null)
    {
        SetState(GroupState.ACTIVE);
        index = id;
        if (member != null)
            AddMember(member);
    }

    public void AddMember(Circumference c)
    {
        if (c.tag == "Rotator")
        {
            AnalyticsSender.SendCustomAnalitycs("wtfError", new Dictionary<string, object>()
                {
                    { "type", "0006" },
                    { "message", "Rotator metido en un grupo" }
                }
            );
        }            
		
        if (!members.Contains(c))
        {
            members.Add(c);
            c.colorGroup = index;
            c.name += "_group" + index;
        }
    }

    public void AddMembers(List<Circumference> membersList)
    {
        foreach (Circumference member in membersList)
        {
            member.name += "_group" + index;
            AddMember(member);
        }
    }

    public bool Contains(Circumference c)
    {
        return members.Contains(c);
    }

    public int Count
    {
        get { return members.Count; }
    }

    public void Erase()
    {
        SetState(GroupState.REMOVE);
        GameManager.Instance.StartCoroutine(DestroyMembers());
    }

    public void SetState(GroupState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }    
    }

    public IEnumerator DestroyMembers(bool sumPoints = true)
    {
        for (int i = members.Count - 1; i >= 0; i--)
        {
            if (members[i] != null)
            {
                Pin pin = members[i].gameObject.GetComponent<Pin>();
                if (sumPoints)
                {
                    pin.pointsValue = 1;
                }
                pin.Autodestroy();
            }
            yield return new WaitForSeconds(Pin.TIME_TO_DESTROY / members.Count);
        }
    }
}