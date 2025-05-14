```
builder.Services.AddFastElasticsearch("db.json", new EsAop());
builder.Services.AddFastOllama("db.json", new OllamaAop());

```

```
//json 
 "Elasticsearch": {
   "Host": [ "http://127.0.0.1:9200" ],
   "UserName": "elastic",
   "PassWord": "123456"
 }
```
![](https://github.com/weizhonzhen/FastKMSWeb.Core/blob/main/img.png)