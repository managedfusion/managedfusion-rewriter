<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel dslVersion="1.0.0.0" Id="7e9de863-fc47-4f6c-b49a-1149320599c4" namespace="ManagedFusion.Rewriter.Configuration" xmlSchemaNamespace="http://managedfusion.com/xsd/managedFusion/rewriter" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <configurationElements>
    <configurationSection name="ManagedFusionRewriterSectionGroup" namespace="ManagedFusion.Rewriter.Configuration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="managedFusion.rewriter">
      <elementProperties>
        <elementProperty name="Rules" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="rules" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/RulesSection" />
          </type>
        </elementProperty>
        <elementProperty name="Rewriter" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="rewriter" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/RewriterSection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="RulesSection" namespace="ManagedFusion.Rewriter.Configuration">
      <attributeProperties>
        <attributeProperty name="Engine" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="engine" isReadOnly="false" defaultValue="RulesEngine.Apache">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/RulesEngine" />
          </type>
        </attributeProperty>
        <attributeProperty name="AllowOutputProcessing" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="allowOutputProcessing" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Apache" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="apache" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/ApacheSection" />
          </type>
        </elementProperty>
        <elementProperty name="Microsoft" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="microsoft" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/MicrosoftSection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="ApacheSection" collectionType="BasicMap" xmlItemName="ruleSet" codeGenOptions="Indexer">
      <attributeProperties>
        <attributeProperty name="DefaultFileName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="defaultFileName" isReadOnly="false" defaultValue="&quot;ManagedFusion.Rewriter.txt&quot;">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="DefaultPhysicalApplicationPath" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="defaultPhysicalApplicationPath" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/ApacheRuleSetItem" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="MicrosoftSection" />
    <configurationElement name="ApacheRuleSetItem">
      <attributeProperties>
        <attributeProperty name="ApplicationPath" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="applicationPath" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="ConfigPath" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="configPath" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="ProxySection">
      <attributeProperties>
        <attributeProperty name="UseAsyncProxy" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="useAsyncProxy" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="BufferSize" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="bufferSize" isReadOnly="false" defaultValue="4096">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="ResponseSize" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="responseSize" isReadOnly="false" defaultValue="2048">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="RequestSize" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="requestSize" isReadOnly="false" defaultValue="2048">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="ProxyType" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="proxyType" isReadOnly="false" defaultValue="&quot;ManagedFusion.Rewriter.ProxyHandler, ManagedFusion.Rewriter&quot;">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="ProxyAsyncType" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="proxyAsyncType" isReadOnly="false" defaultValue="&quot;ManagedFusion.Rewriter.ProxyAsyncHandler, ManagedFusion.Rewriter&quot;">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="RewriterSection">
      <attributeProperties>
        <attributeProperty name="TraceLog" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="traceLog" isReadOnly="false" defaultValue="false">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="RebaseClientPath" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="rebaseClientPath" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="AllowIis7TransferRequest" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="allowIis7TransferRequest" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="AllowVanityHeader" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="allowVanityHeader" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="AllowXRewriteUrlHeader" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="allowXRewriteUrlHeader" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="AllowRequestHeaderAdding" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="allowRequestHeaderAdding" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="AllowServerVariableSetting" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="allowServerVariableSetting" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="ModifyIIS7WorkerRequest" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="modifyIIS7WorkerRequest" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Proxy" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="proxy" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/ProxySection" />
          </type>
        </elementProperty>
        <elementProperty name="Modules" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="modules" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/RewriterModulesSection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="RewriterModulesSection" collectionType="AddRemoveClearMap" xmlItemName="add" codeGenOptions="Indexer">
      <itemType>
        <configurationElementMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/RewriterModuleItem" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="RewriterModuleItem">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Type" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="type" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7e9de863-fc47-4f6c-b49a-1149320599c4/Type" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
    <externalType name="RulesEngine" namespace="ManagedFusion.Rewriter.Configuration" />
    <externalType name="Type" namespace="System" />
  </typeDefinitions>
</configurationSectionModel>