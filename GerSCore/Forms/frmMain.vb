﻿Imports System.IO
Public Class frmMain

    '####################################################################################################
    'Deklaration
    '####################################################################################################

    Private WithEvents _project As clsProjekt
    Private _usedcolors As New List(Of Color)
    Dim fs As FileStream

    Public DebugPrefix As Integer = 0
    Public rnd As New Random

    '####################################################################################################
    'Konstruktoren
    '####################################################################################################

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Debug.Print(StrDup(75, "-"))
        DebugPrefix += 1 : Debug.Print(StrDup(DebugPrefix, "+") & " " & "Enter in: {0} Sub ->  {1}", "frmMain", "Load")

        MenuRecentProjectsInit()
        Randomize()

        Debug.Print(StrDup(DebugPrefix, "+") & " " & "Leave in: {0} Sub ->  {1}", "frmMain", "Load") : DebugPrefix -= 1
    End Sub

    '####################################################################################################
    'Methoden
    '####################################################################################################

    Private Sub openProject(Optional ByVal Path As String = "")
        If Path = "" Then
            dlgOpen.FileName = ""
            dlgOpen.Filter = "GerSCore Projekte (*.gsProj)|*.gsProj"
            If IO.Directory.Exists(My.Settings.RecentPath) Then dlgOpen.InitialDirectory = My.Settings.RecentPath
            If dlgOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
                Path = dlgOpen.FileName
            Else
                Exit Sub
            End If
        End If
        If IO.File.Exists(Path) Then
            Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            fs = New FileStream(Path, FileMode.Open)
            Debug.Print(Path.ToString)
            _project = CType(bf.Deserialize(fs), clsProjekt)
            _project.Project_initiated(True)
            Debug.Print("geladen")
            fs.Close()
            My.Settings.RecentPath = Path
            MenuRecentProjects_Add(Path)
        Else
            MsgBox("Der Pfad wurde nicht gefunden!", MsgBoxStyle.Critical, "Projekt Öffnen gescheitert!")
        End If
    End Sub

    Private Sub Tests_init()
        DebugPrefix += 1 : Debug.Print(StrDup(DebugPrefix, "+") & " " & "Enter in: {0} Sub ->  {1}", "frmMain", "Tests_init")

        Me.NeuToolStripMenuItem.PerformClick()

        Project.AddPart("Testpart1")
        Project.AddPart("Testpart2")
        Project.AddPart("Testpart3")
        Project.Parts(0).AddGerber("Testgerber1")
        Project.Parts(0).AddGerber("Testgerber2")
        Project.Parts(1).AddGerber("Testgerber1")
        Project.Parts(1).AddGerber("Testgerber2")
        Project.Parts(2).AddGerber("Testgerber1")
        Project.Parts(2).AddGerber("Testgerber2")

        Project.Parts(0).Gerber(0).Shapes.Add(New clsLine(Project.Parts(0).Gerber(0), New Point(10, 10), New Point(850 - 30, 500)))
        Project.Parts(0).Gerber(0).Shapes.Add(New clsLine(Project.Parts(0).Gerber(0), New Point(0, 0), New Point(935, 0)))
        Project.Parts(0).Gerber(0).Shapes.Add(New clsLine(Project.Parts(0).Gerber(0), New Point(0, 0), New Point(0, 508)))
        Project.Parts(0).Gerber(0).Shapes.Add(New clsLine(Project.Parts(0).Gerber(0), New Point(0, 0), New Point(939, 514)))
        Project.Parts(0).Gerber(0).Shapes.Add(New clsLine(Project.Parts(0).Gerber(0), New Point(0, 514), New Point(939, 0)))
        Project.Parts(0).Gerber(1).Shapes.Add(New clsLine(Project.Parts(0).Gerber(1), New Point(60, 10), New Point(850 - 60, 500)))
        Project.Parts(1).Gerber(0).Shapes.Add(New clsLine(Project.Parts(1).Gerber(0), New Point(90, 10), New Point(850 - 90, 500)))
        Project.Parts(1).Gerber(1).Shapes.Add(New clsLine(Project.Parts(1).Gerber(1), New Point(120, 10), New Point(850 - 120, 500)))
        Project.Parts(2).Gerber(0).Shapes.Add(New clsLine(Project.Parts(2).Gerber(0), New Point(150, 10), New Point(850 - 150, 500)))
        Project.Parts(2).Gerber(1).Shapes.Add(New clsLine(Project.Parts(2).Gerber(1), New Point(180, 10), New Point(850 - 180, 500)))

        Debug.Print(StrDup(DebugPrefix, "+") & " " & "Leave in: {0} Sub ->  {1}", "frmMain", "Tests_init") : DebugPrefix -= 1
    End Sub

    Protected Friend Sub MenuRecentProjectsInit()
        With My.Settings
            If IsNothing(.RecentProjects) Then .RecentProjects = New Specialized.StringCollection
            RecentProjectsToolStripMenuItem.DropDownItems.Clear()
            If .RecentProjects.Count = 0 Then
                RecentProjectsToolStripMenuItem.DropDownItems.Add("<...nichts...>")
                Exit Sub
            End If
            For Each Element In .RecentProjects
                If Not IO.File.Exists(Element) Then .RecentProjects.Remove(Element)
            Next
            If .RecentProjects.Count > 10 Then
                For i = .RecentProjects.Count To 11 Step -1
                    .RecentProjects.RemoveAt(i - 1)
                Next
            End If
            RecentProjectsToolStripMenuItem.DropDownItems.Clear()
            For Each Element In .RecentProjects
                Dim Item As New ToolStripMenuItem(Element)
                AddHandler Item.Click, AddressOf RecentProject_Click
                RecentProjectsToolStripMenuItem.DropDownItems.Add(Item)
            Next
        End With
    End Sub

    Protected Friend Sub MenuRecentProjects_Add(ByVal Path As String)
        With My.Settings
            If .RecentProjects.Contains(Path) Then
                .RecentProjects.Remove(Path)
            End If
            .RecentProjects.Insert(0, Path)
        End With
        MenuRecentProjectsInit()
    End Sub

    '####################################################################################################
    'Funktionen
    '####################################################################################################

    '####################################################################################################
    'Property
    '####################################################################################################

    Friend ReadOnly Property Project As clsProjekt
        Get
            Return _project 'clsProjekt.Instance
        End Get
    End Property

    Friend Property UsedColor As List(Of Color)
        Get
            Return _usedcolors
        End Get
        Set(value As List(Of Color))
            _usedcolors = value
        End Set
    End Property

    '####################################################################################################
    'Events
    '####################################################################################################

    Private Sub PartBibliothekToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PartBibliothekToolStripMenuItem.Click
        frmPartLib.ShowDialog()
    End Sub

    Private Sub BeendenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BeendenToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub NeuToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NeuToolStripMenuItem.Click
        _project = clsProjekt.Instance
    End Sub

    Private Sub SpeichernToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpeichernToolStripMenuItem.Click
        _project.Save(Project.Path)
    End Sub

    Private Sub SpeichernunterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpeichernunterToolStripMenuItem.Click
        _project.Save()
    End Sub

    Private Sub TestsInitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestsInitToolStripMenuItem.Click
        Tests_init()
    End Sub

    Private Sub ÖffnenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ÖffnenToolStripMenuItem.Click
        openProject()
    End Sub

    Private Sub RecentProject_Click(Sender As Object, e As EventArgs)
        openProject(CType(Sender, ToolStripMenuItem).Text)
    End Sub

End Class
