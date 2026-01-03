
Imports System
Imports System.Collections.Generic
Imports System.IO

Module LibrarySystem

    Const DATA_FILE As String = "library_books_vb.txt"

    ' Dictionary to store books
    Dim books As New Dictionary(Of String, Book)()

    ' Queue for loan requests
    Dim loanQueue As New Queue(Of String)()

    ' Stack for last returned books
    Dim returnStack As New Stack(Of String)()

    ' Struct for book summary
    Structure BookSummary
        Public Title As String
        Public AvailableCopies As Integer
    End Structure

    Class Book
        Public Title As String
        Public TotalCopies As Integer
        Public AvailableCopies As Integer
    End Class

    Sub AddBook()
        Console.Write("Book ID: ")
        Dim id As String = Console.ReadLine()
        Console.Write("Book Title: ")
        Dim title As String = Console.ReadLine()
        Console.Write("Total Copies: ")
        Dim total As Integer = Integer.Parse(Console.ReadLine())

        books(id) = New Book With {.Title = title, .TotalCopies = total, .AvailableCopies = total}
        Console.WriteLine("Book added successfully.")
    End Sub

    Sub LoanBook()
        Console.Write("Book ID to loan: ")
        Dim id As String = Console.ReadLine()

        If Not books.ContainsKey(id) Then
            Console.WriteLine("Book not found.")
            Return
        End If

        If books(id).AvailableCopies <= 0 Then
            Console.WriteLine("No copies available. Adding to loan queue.")
            loanQueue.Enqueue(id)
            Return
        End If

        books(id).AvailableCopies -= 1
        Console.WriteLine($"Book '{books(id).Title}' loaned successfully.")
    End Sub

    Sub ReturnBook()
        Console.Write("Book ID to return: ")
        Dim id As String = Console.ReadLine()

        If Not books.ContainsKey(id) Then
            Console.WriteLine("Book not found.")
            Return
        End If

        books(id).AvailableCopies += 1
        returnStack.Push(id)
        Console.WriteLine($"Book '{books(id).Title}' returned successfully.")

        ' Process waiting loan queue
        If loanQueue.Count > 0 Then
            Dim queuedId As String = loanQueue.Peek()
            If books(queuedId).AvailableCopies > 0 Then
                loanQueue.Dequeue()
                books(queuedId).AvailableCopies -= 1
                Console.WriteLine($"Queued loan processed: '{books(queuedId).Title}' loaned.")
            End If
        End If
    End Sub

    Sub GenerateReport()
        Console.WriteLine(vbCrLf & "Library Inventory Report:")
        For Each kvp In books
            Dim summary As New BookSummary With {
                .Title = kvp.Value.Title,
                .AvailableCopies = kvp.Value.AvailableCopies
            }
            Console.WriteLine($"Book ID: {kvp.Key}, Title: {summary.Title}, Available: {summary.AvailableCopies}/{kvp.Value.TotalCopies}")
        Next
    End Sub

    Sub ViewLastReturnedBook()
        If returnStack.Count = 0 Then
            Console.WriteLine("No returned books yet.")
            Return
        End If

        Dim lastId As String = returnStack.Peek()
        Console.WriteLine($"Last returned book: {books(lastId).Title}")
    End Sub

    Sub Save()
        Using sw As New StreamWriter(DATA_FILE)
            For Each kvp In books
                sw.WriteLine($"{kvp.Key}|{kvp.Value.Title}|{kvp.Value.TotalCopies}|{kvp.Value.AvailableCopies}")
            Next
        End Using
    End Sub

    Sub Load()
        If Not File.Exists(DATA_FILE) Then Return

        For Each line In File.ReadAllLines(DATA_FILE)
            Dim parts() As String = line.Split("|"c)
            books(parts(0)) = New Book With {
                .Title = parts(1),
                .TotalCopies = Integer.Parse(parts(2)),
                .AvailableCopies = Integer.Parse(parts(3))
            }
        Next
    End Sub

    Sub Main()
        Load()
        While True
            Console.WriteLine(vbCrLf & "1 Add Book" &
                              vbCrLf & "2 Loan Book" &
                              vbCrLf & "3 Return Book" &
                              vbCrLf & "4 Inventory Report" &
                              vbCrLf & "5 View Last Returned" &
                              vbCrLf & "6 Exit")

            Dim choice As String = Console.ReadLine()
            Select Case choice
                Case "1"
                    AddBook()
                Case "2"
                    LoanBook()
                Case "3"
                    ReturnBook()
                Case "4"
                    GenerateReport()
                Case "5"
                    ViewLastReturnedBook()
                Case "6"
                    Save()
                    Exit While
                Case Else
                    Console.WriteLine("Invalid option.")
            End Select
        End While
    End Sub
End Module
