Imports <xmlns:itunes="http://www.itunes.com/dtds/podcast-1.0.dtd">
Public Class FeedConverter

    Public Shared Function Create(host As Uri, feed As Object, posts As IEnumerable(Of Object)) As String
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
        <link><%= host.ToString() & feed.PartitionKey & "/" & feed.RowKey %></link>
        <itunes:author><%= feed.PartitionKey %></itunes:author>
        <copyright>None</copyright>
        <pubDate>Tue, 01 Jan 2013 09:00:00 -0300</pubDate>
        <lastBuildDate>Tue, 01 Jan 2013 09:00:00 -0300</lastBuildDate>
        <generator>Recast</generator>
        <language>pt-BR</language>
        <managingEditor><%= feed.PartitionKey %></managingEditor>
        <webMaster><%= feed.PartitionKey %></webMaster>
        <itunes:owner>
            <itunes:name><%= feed.PartitionKey %></itunes:name>
            <itunes:email>someemail@email.com</itunes:email>
        </itunes:owner>
        <ttl>90</ttl>
        <itunes:image href=<%= host.ToString() & "someimage.jpg" %>/>
        <image>
            <url><%= host.ToString() %>someimage.jpg</url>
            <title><%= feed.RowKey %></title>
            <link>host.ToString()</link>
            <description>
                A recasted podcast.
            </description>
        </image>
        <docs>http://www.rssboard.org/rss-specification</docs>
        <itunes:keywords>recast</itunes:keywords>
        <category>Other</category>
        <itunes:category text="Other">
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
            <link><%= post.SongUrl %></link>
            <guid><%= post.SongUrl %></guid>
            <itunes:author><%= feed.PartitionKey %></itunes:author>
            <enclosure url=<%= post.SongUrl %> length="-1" type="audio/mpeg"/>
            <itunes:duration>00:01:00</itunes:duration>
            <author><%= feed.PartitionKey %></author>
            <pubDate>Tue, 01 Jan 2013 10:00:00 -0300</pubDate>
            <category>Other</category>
            <itunes:block>no</itunes:block>
            <itunes:explicit>no</itunes:explicit>
            <itunes:keywords>
                other
            </itunes:keywords>
        </item>
        %>

    </channel>
</rss>

        Return xml.ToString()
    End Function

End Class
