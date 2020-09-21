
Imports System.Data
Imports System.Data.SqlClient
Imports Nest
Imports System.IO
Imports System
Imports System.Text
Imports System.Net


Public Class Form1
    Private tabla As DataTable
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Conectar()
    End Sub

    Public Class Persona
        Public Property nom As String
        Public Property cognom1 As String
        Public Property cognom2 As String
    End Class

    Private Function Conectar() As Boolean
        Dim sCnn As String
        sCnn = "Data Source=31.187.68.50;Initial Catalog=HestiaTEST;User ID=desarrollo;password=CAT365adminpass2018"

        Dim sSel As String = "SELECT top 2 * FROM Apersona"

        Dim da As SqlDataAdapter
        Dim dt As New DataTable

        Try
            da = New SqlDataAdapter(sSel, sCnn)
            da.Fill(dt)
            tabla = dt
            Me.DataGridView1.DataSource = dt
            LabelInfo.Text = String.Format("Total datos en la tabla: {0}", dt.Rows.Count)
            Return True
        Catch ex As Exception
            LabelInfo.Text = "Error: " & ex.Message
            Return False
        End Try
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim node As New Uri("http://localhost:9200")
        Dim settings = New ConnectionSettings(node)
        settings.DefaultIndex("personas")
        'settings.DefaultIndex("stackoverflow")
        Dim client = New ElasticClient(settings)
        For Each row As DataRow In tabla.Rows
            Dim pers As New Persona
            With pers
                .nom = row.Item("Nom")
                .cognom1 = row.Item("Cognom1")
                .cognom2 = row.Item("Cognom2")

            End With

            Dim indexResponse = client.IndexDocument(pers)
            If indexResponse.IsValid Then
                MessageBox.Show("Guardado OK")
            End If
        Next
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim node As New Uri("http://localhost:9200")

        Dim settings As New ConnectionSettings(node)
        settings.DefaultIndex("personas")
        'settings.ThrowExceptions(True)
        'settings.PrettyJson()

        Dim client As New ElasticClient(settings)


        Dim sr As SearchRequest(Of Persona) = New SearchRequest(Of Persona)
        'sr.Query = New MatchAllQuery
        sr.From() = 0
        Dim mq = New MatchQuery

        mq.Field = "nom"
        mq.Query = "Isaac"
        Dim mq2 = New MatchQuery

        mq2.Field = "nom"
        mq2.Query = "Edu"

        'per ordenar per nom
        Dim LSort As New List(Of ISort)
        Dim fs As New FieldSort

        fs.Field = "nom"
        fs.Order = Nest.SortOrder.Descending
        LSort.Add(fs)

        sr.Sort = LSort

        'fi ordenacio

        'fem el or dels camps
        sr.Query = mq Or mq2


        Dim response As SearchResponse(Of Persona) = New SearchResponse(Of Persona)

        response = client.Search(Of Persona)(sr)




        For Each doc In response.Hits
            MessageBox.Show(doc.Source.nom & " " & doc.Source.cognom1 & " " & doc.Source.cognom2)
        Next



        'Dim url_req As String = "http://localhost:9200/personas/_search?pretty"

        'Dim s As String = "{\""query\"": {\""match\"": { \""nom\"" : \""Isaac\"" }}}"

        'Using cliente2 = New WebClient()
        '    cliente2.UploadString(url_req, "POST", s)
        'End Using



        '|| New MatchQuery { Field = "description", Query = "nest" }





        '  Dim response2 As SearchResponse(Of Persona) = client.Search(Of Persona)()









        Dim json4 = "{
                        ""personas"":{
                        ""query"": {
                             ""nom"" : {
                                ""value"" : ""Isaac""
                                }
                         }
                       }
                    }"

        '"{
        '    ""query"": {
        '        ""fuzzy"": { 
        '            ""title"": {
        '                ""value"": ""potato""
        '            }
        '        }
        '    }
        '}"

        Dim SearchRequest As SearchRequest = New SearchRequest


        Using stream = New MemoryStream(Encoding.UTF8.GetBytes(json4))
            SearchRequest = client.SourceSerializer.Deserialize(Of SearchRequest)(stream)
        End Using


        Dim response2 As SearchResponse(Of Persona) = New SearchResponse(Of Persona)

        response2 = client.Search(Of Persona)(SearchRequest)



        'For Each doc In response.Hits
        '    MessageBox.Show(doc.Source.nom & " " & doc.Source.cognom1 & " " & doc.Source.cognom2)
        'Next


    End Sub
End Class
