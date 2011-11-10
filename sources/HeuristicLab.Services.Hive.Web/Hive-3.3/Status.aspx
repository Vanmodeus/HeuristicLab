﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Status.aspx.cs" Inherits="Status" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
  Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">
  <div>
    Available Cores:
    <asp:Label ID="availableCoresLabel" runat="server" />
    <br />
    Used Cores / Calculating Jobs:
    <asp:Label ID="usedCoresLabel" runat="server" />
    <br />
    Waiting Jobs:
    <asp:Label ID="waitingJobsLabel" runat="server" />
    <br />
    Slaves (CPU Utilization):
    <asp:Label ID="slavesLabel" runat="server" />
    <br />
    Avg. CPU Utilization:
    <asp:Label ID="cpuUtilizationLabel" runat="server" />
    <br />
    ExecutionTime on Hive:
    <asp:Label ID="totalExecutionTimeLabel" runat="server" />
    <br />
    <br />
    Days:
    <asp:DropDownList ID="daysDropDownList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="daysDropDownList_SelectedIndexChanged">
      <asp:ListItem Value="1"></asp:ListItem>
      <asp:ListItem Value="2"></asp:ListItem>
      <asp:ListItem Value="3"></asp:ListItem>
      <asp:ListItem Value="4"></asp:ListItem>
      <asp:ListItem Value="5"></asp:ListItem>
      <asp:ListItem Value="6"></asp:ListItem>
      <asp:ListItem Value="7"></asp:ListItem>
      <asp:ListItem Value="8"></asp:ListItem>
      <asp:ListItem Value="9"></asp:ListItem>
      <asp:ListItem Value="10"></asp:ListItem>
      <asp:ListItem Value="11"></asp:ListItem>
      <asp:ListItem Value="12"></asp:ListItem>
      <asp:ListItem Value="13"></asp:ListItem>
      <asp:ListItem Value="14"></asp:ListItem>
      <asp:ListItem Value="All"></asp:ListItem>
    </asp:DropDownList>
    <br />
    <br />
    Avg. CPU Utilization History of all Slaves<br />
    <asp:Chart ID="cpuUtilizationChart" runat="server" Height="270px" Width="1900px">
      <Series>
        <asp:Series BorderWidth="2" ChartType="Line" Color="0, 176, 80" Name="Series1" XValueType="DateTime"
          YValueType="Double">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY>
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False" >
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
    <br />
    Cores/Used Cores History<br />
    <asp:Chart ID="coresChart" runat="server" Palette="None" Width="1900px" PaletteCustomColors="137, 165, 78; 185, 205, 150">
      <Series>
        <asp:Series ChartType="Area" Name="Cores" XValueType="DateTime" YValueType="Double">
        </asp:Series>
        <asp:Series ChartArea="ChartArea1" ChartType="Area" Name="FreeCores" XValueType="DateTime"
          YValueType="Double">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY>
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False" >
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
    <br />
    Memory/Used Memory History (GB)<br />
    <asp:Chart ID="memoryChart" runat="server" Palette="None" PaletteCustomColors="170, 70, 67; 209, 147, 146"
      Width="1900px">
      <Series>
        <asp:Series ChartType="Area" Name="Cores" XValueType="DateTime" YValueType="Double">
        </asp:Series>
        <asp:Series ChartArea="ChartArea1" ChartType="Area" Name="FreeCores" XValueType="DateTime"
          YValueType="Double">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY>
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False" >
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
    <br />
    Speedup (ComputedMinutes/Minute)<br />
    <asp:Chart ID="speedupChartMinutes" runat="server" Palette="None" Width="1900px"
      PaletteCustomColors="79, 129, 189">
      <Series>
        <asp:Series ChartType="Area" Name="Speedup" XValueType="DateTime" YValueType="Double">
        </asp:Series>
        <asp:Series BorderWidth="2" ChartArea="ChartArea1" 
          ChartType="Line" Color="185, 205, 150" Name="Cores">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY Minimum="-5">
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False" >
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
    <br />
    Speedup (ComputedHours/Hour)<br />
    <br />
    <asp:Chart ID="speedupChartHours" runat="server" Palette="None" Width="1900px" PaletteCustomColors="79, 129, 189">
      <Series>
        <asp:Series ChartType="Area" Name="Speedup" XValueType="DateTime" 
          YValueType="Double">
        </asp:Series>
        <asp:Series BorderWidth="2" ChartArea="ChartArea1" 
          ChartType="Line" Color="185, 205, 150" Name="Cores">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY>
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False">
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
    <br />
  </div>
  </form>
</body>
</html>
