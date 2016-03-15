
using System.Xml;
using System;

/// <summary>
///XmlHelper类，用于处理Xml文档的相关操作
/// </summary>
public class XmlHelper
{
    /// <summary>
    /// 读取Xml文档
    /// 使用示列:
    /// XmlHelper.XmlDocument(path);
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="node">节点名称</param>
    /// <param name="attribute">特性名称</param>
    /// <returns>string</returns>
    public static XmlDocument ReadXmlNode(string path)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return doc;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 读取Xml文档节点数据
    /// 使用示列:
    /// XmlHelper.ReadXmlNode(path, "nodeName");
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="node">节点名称</param>
    /// <param name="attribute">特性名称</param>
    /// <returns>string</returns>
    public static XmlNode ReadXmlNode(string path, string nodeName)
    {
        XmlNode value = null;

        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            value = doc.SelectSingleNode(nodeName);
        }
        catch
        { }

        return value;
    }

    /// <summary>
    /// 读取Xml文档节点数据
    /// 使用示列:
    /// XmlHelper.ReadXmlNodeText(path, "node", "");
    /// XmlHelper.ReadXmlNodeText(path, "node", "attribute");
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="node">节点名称</param>
    /// <param name="attribute">特性名称</param>
    /// <returns>string</returns>
    public static string ReadXmlNodeText(string path, string node, string attribute)
    {
        string value = "";

        try
        {
            XmlDocument doc = new XmlDocument();
            if (!System.IO.File.Exists(path))
            {
                return value;
            }
            doc.Load(path);
            XmlNode no = doc.SelectSingleNode(node);
            value = (string.IsNullOrEmpty(attribute)) ? no.InnerText : no.Attributes[attribute].Value;
        }
        catch
        { }

        return value;
    }

    /// <summary>
    /// 设置XmlNode属性
    /// 使用示列:
    /// XmlHelper.SetAttribute(path, nodeName, "");
    /// XmlHelper.SetAttribute(path, nodeName, attributeName,attributeValue);
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="nodeName">节点名称</param>
    /// <param name="attributeName">属性名称</param>
    /// <param name="attributeValue">属性值</param>
    /// <returns>void</returns>
    public static void SetAttribute(string path, string nodeName, string attributeName, string attributeValue)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlElement element = doc.SelectSingleNode(nodeName) as XmlElement;
            if (element == null)
            {
                throw new Exception("节点元素不存在！");
            }
            else
            {
                if (attributeName.Equals(""))
                {
                    element.InnerText = attributeValue;
                }
                else
                {
                    element.SetAttribute(attributeName, attributeValue);
                }
                doc.Save(path);
            }
        }
        catch
        { }
    }

    /// <summary>
    /// 删除XmlNode属性
    /// 使用示列:
    /// XmlHelper.DeleteAttribute(path, nodeName, "");
    /// XmlHelper.DeleteAttribute(path, nodeName, attributeName,attributeValue);
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="nodeName">节点名称</param>
    /// <param name="attributeName">属性名称</param>
    /// <returns>void</returns>
    public static void DeleteAttribute(string path, string nodeName, string attributeName)
    {
        if (attributeName.Equals(""))
        {
            return;
        }

        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlElement element = doc.SelectSingleNode(nodeName) as XmlElement;
            if (element == null)
            {
                throw new Exception("节点元素不存在！");
            }
            else
            {
                element.RemoveAttribute(attributeName);
                doc.Save(path);
            }
        }
        catch
        { }
    }

    /// <summary>
    /// 读取XmlNode的所以子节点
    /// 使用示列:
    /// XmlHelper.ReadNodeList(path, nodeName);
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="nodeName">节点名称</param>
    /// <returns>XmlNodeList</returns>
    public static XmlNodeList ReadNodeList(string path, string nodeName)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode element = doc.SelectSingleNode(nodeName);
            if (element == null)
            {
                throw new Exception("节点元素不存在！");
            }
            else
            {
                return element.ChildNodes;
            }
        }
        catch
        {
            return null;
        }
    }

    public static void CreateXmlFile(string path, string rootName)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            //建立Xml的定义声明   
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);   
            doc.AppendChild(dec);   
            //创建根节点   
            XmlElement root = doc.CreateElement(rootName);   
            doc.AppendChild(root);   
            doc.Save(path);
        }
        catch
        {
        }
    }

    public static void AddNode(string path, string nodeName, string childNodeName, string nodeValue)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlElement element = doc.SelectSingleNode(nodeName) as XmlElement;
            if (element == null)
            {
                throw new Exception("节点元素不存在！");
            }
            else
            {
                XmlElement childNode = doc.CreateElement(childNodeName);
                childNode.InnerText = nodeValue;
                element.AppendChild(childNode);
                doc.Save(path);
            }
        }
        catch
        { }
    }
}