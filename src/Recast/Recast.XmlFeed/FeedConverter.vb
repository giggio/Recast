Imports <xmlns:itunes="http://www.itunes.com/dtds/podcast-1.0.dtd">
Public Class FeedConverter

    Public Shared Function Create(host As Uri, feed As Object, posts As IEnumerable(Of Object)) As String
        Dim lastDate As DateTime
        For Each post In posts
            If post.PublishDate > lastDate Then
                lastDate = post.PublishDate
            End If
        Next
        Dim xml =
<rss xmlns:itunes="http://www.itunes.com/dtds/podcast-1.0.dtd" xmlns:atom="http://www.w3.org/2005/Atom" version="2.0">
    <channel>
        <title><%= feed.RowKey %></title>
        <itunes:subtitle></itunes:subtitle>
        <description>
            A recasted podcast
        </description>
        <itunes:summary>
            No Summary
        </itunes:summary>
        <link><%= host.ToString() & "Feeds/" & feed.PartitionKey & "/" & feed.RowKey %></link>
        <itunes:author><%= feed.PartitionKey %></itunes:author>
        <copyright>None</copyright>
        <pubDate><%= lastDate.ToString("R") %></pubDate>
        <lastBuildDate><%= lastDate.ToString("R") %></lastBuildDate>
        <generator>Recast</generator>
        <language>pt-BR</language>
        <managingEditor><%= feed.PartitionKey %></managingEditor>
        <webMaster><%= feed.PartitionKey %></webMaster>
        <itunes:owner>
            <itunes:name><%= feed.PartitionKey %></itunes:name>
            <!--<itunes:email>someemail@email.com</itunes:email>-->
        </itunes:owner>
        <itunes:image href=<%= host.ToString() & "Images/recast_logo.png" %>/>
        <image>
            <url><%= host.ToString() & "Images/recast_logo.png" %></url>
            <title><%= feed.RowKey %></title>
            <link><%= host.ToString() & "Feeds/" & feed.PartitionKey & "/" & feed.RowKey %></link>
            <description>A recasted podcast</description>
        </image>
        <docs>http://www.rssboard.org/rss-specification</docs>
        <itunes:keywords>recast</itunes:keywords>
        <category>Technology</category>
        <itunes:category text="Technology">
            <itunes:category text="Podcasting"/>
        </itunes:category>
        <itunes:block>no</itunes:block>
        <itunes:explicit>no</itunes:explicit>
        <atom:link href=<%= host.ToString() & "feed" %> rel="self" type="application/rss+xml"/>
        <%= From post In posts
            Select
        <item>
            <title><%= post.Title %></title>
            <description><%= post.Description %></description>
            <itunes:subtitle><%= post.Subtitle %></itunes:subtitle>
            <itunes:summary><%= post.Summary %></itunes:summary>
            <link><%= host.ToString() & "Feeds/" & feed.PartitionKey & "/" & feed.RowKey %></link>
            <guid><%= post.SongLink %></guid>
            <itunes:author><%= feed.PartitionKey %></itunes:author>
            <enclosure url=<%= post.SongLink %> type="audio/mpeg"/>
            <itunes:duration><%= post.Duration %></itunes:duration>
            <author><%= feed.PartitionKey %></author>
            <pubDate><%= DirectCast(post.PublishDate, DateTime).ToString("R") %></pubDate>
            <category>Other</category>
            <itunes:block>no</itunes:block>
            <itunes:explicit>no</itunes:explicit>
            <itunes:keywords>
                recast
            </itunes:keywords>
        </item>
        %>

    </channel>
</rss>

        Return xml.ToString()
    End Function

End Class
