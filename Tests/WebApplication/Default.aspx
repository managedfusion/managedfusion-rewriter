<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Default Test page</title>
    <style media="screen" type="text/css">
		th {
			background-color: blue;
			font-weight: bold;
			color: white;
		}
    </style>
</head>
<body>
<table>
	<thead>
		<tr>
			<th colspan="2">Headers</th>
		</tr>
		<tr>
			<th>Key</th>
			<th>Value</th>
		</tr>
	</thead>
	<tbody>
	<% foreach(string key in this.Request.Headers.AllKeys) { %>
		<tr>
			<td><%= key %></td>
			<td><%= this.Request.Headers[key] %></td>
		</tr>
	<% } %>
	</tbody>
</table>
<table>
	<thead>
		<tr>
			<th colspan="2">Server Variables</th>
		</tr>
		<tr>
			<th>Key</th>
			<th>Value</th>
		</tr>
	</thead>
	<tbody>
	<% foreach(string key in this.Request.ServerVariables.AllKeys) { %>
		<tr>
			<td><%= key %></td>
			<td><%= this.Request.ServerVariables[key]%></td>
		</tr>
	<% } %>
	</tbody>
</table>
<table>
	<thead>
		<tr>
			<th colspan="2">Query String</th>
		</tr>
		<tr>
			<th>Key</th>
			<th>Value</th>
		</tr>
	</thead>
	<tbody>
	<% foreach(string key in this.Request.QueryString.AllKeys) { %>
		<tr>
			<td><%= key %></td>
			<td><%= this.Request.QueryString[key]%></td>
		</tr>
	<% } %>
	</tbody>
</table>
<table>
	<thead>
		<tr>
			<th colspan="2">Form</th>
		</tr>
		<tr>
			<th>Key</th>
			<th>Value</th>
		</tr>
	</thead>
	<tbody>
	<% foreach(string key in this.Request.Form.AllKeys) { %>
		<tr>
			<td><%= key %></td>
			<td><%= this.Request.Form[key]%></td>
		</tr>
	<% } %>
	</tbody>
</table>

</body>
</html>
