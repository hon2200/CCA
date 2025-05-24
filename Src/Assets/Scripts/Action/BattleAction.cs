using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// �Ƿ��ͻ���
public abstract class ActionBase : ICloneable
{
    public ActionDefine ActionInfo { get; protected set; }
    public int Target { get; set; }
    //��ʱ���ֵֻ�ڹ����ж�����ʹ�ã��ж��ж��Ƿ���Ч��δ�����������ж��������õ�
    public bool isEffective { get; set; }
    public abstract object Clone();
}

// ����ʵ��
public class BattleAction<T> : ActionBase where T : ActionDefine
{
    //ע�⣬��BattleAction��ȡActioninfo�������Ϣ��ʱ��ʹ�õ������
    //�����Ĳ���ʵ�����û����ActionInfo����ʵ�ϣ���Ϊ�ֶΣ��������ActionInfo��Ϊ���ԡ�
    //��������ʹ�û���Protected Set����ʵ�ϸо�δ���б�Ҫ����Ϊδ������һЩӢ�۵Ļ������Ի�䣬����costʲô�ģ���Ȼ����ͨ��������ģ�������ʱ����ܲ�̫���㣬���Protected Set���Ŵ���
    //�������ǵ������setֱ��ȥ�޸�base��Action info��getֱ�ӻ��base��Actioninfo��Ҳ��ʵ������������ActionInfo��ȫһ�¡�
    public new T ActionInfo
    {
        get => (T)base.ActionInfo;
        set => base.ActionInfo = value;
    }

    public BattleAction(T actionInfo, int target)
    {
        ActionInfo = actionInfo;
        Target = target;
        isEffective = false;
    }
    public BattleAction(T actionInfo, int target,bool isEffective_)
    {
        ActionInfo = actionInfo;
        Target = target;
        isEffective = isEffective_;
    }
    public override object Clone()
    {
        // �����µ� BattleAction<T>������������ֶ�
        return new BattleAction<T>(
            (T)this.ActionInfo.Clone(), // �ٴ���� ActionInfo
            this.Target,
            this.isEffective
        );
    }
}
