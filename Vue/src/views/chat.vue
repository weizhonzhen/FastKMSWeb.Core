<template>
    <div class="form-inline">
        <div class="form-group">
            <div class="form-group">
                <button @click="chatKmsWin('知识库')" class="btn btn-primary">对话知识库</button>
                <button @click="chatBusinessWin('数据库')" class="btn btn-primary">对话数据库</button>
                <button @click="chatMcpWin('Mcp')" class="btn btn-primary">对话Mcp</button>
            </div>
        </div>
    </div>
    <section class="content">
        <div class="box-body table-responsive"> 
            <table id="kmsTable" class="table table-bordered TableList">
                <thead style="background-color:#cacaca;">
                    <tr>
                        <td width="40%">对话名称</td>
                        <td width="15%">对话次数</td>
                        <td width="15%">对话时间</td>
                        <td width="15%">类型</td>
                        <td width="15%">操作</td>
                    </tr>
                </thead>
                <tbody>
                     <tr v-for=" item in pageData.list">
                        <td width="40%">{{ item.name }}</td>
                        <td width="15%">{{ item.total }}</td>
                        <td width="15%">{{ item.beginTime }}</td>
                        <td width="15%">{{ item.isNL2Sql == 'True' ? "数据库":((item.mcp!=undefined && item.mcp != ''&&JSON.parse(item.mcp).length>0)?"mcp":"知识库") }}</td>
                        <td width="15%">
                           <button class="btn btn-primary btn-sm" @click="Option(item)">查看</button>
                             <button class="btn btn-primary btn-sm" @click="Delete(item)">删除</button>
                         </td>
                    </tr>
                </tbody>
            </table>
            <page v-model:data="pageData.page" v-model:list="pageData.list"/>
        </div>
    </section>
    <el-dialog :title="showTitle" v-model="isShowChatKms" :close-on-click-modal="false" :destroy-on-close="true" 
            :close-on-press-escape ="false" :fullscreen="false">
        <chatKms :data="propsData" />
    </el-dialog>
    <el-dialog :title="showTitle" v-model="isShowBusiness" :close-on-click-modal="false" :destroy-on-close="true" 
            :close-on-press-escape ="false" :fullscreen="false">
        <chatBusiness :data="propsData" />
    </el-dialog>
     <el-dialog :title="showTitle" v-model="isShowMcp" :close-on-click-modal="false" :destroy-on-close="true" 
            :close-on-press-escape ="false" :fullscreen="false">
        <chatMcp :data="propsData" />
    </el-dialog>   
</template>
<style scoped>
    table tr td{text-align: center;}
    button{margin-left:10px;}
</style>
<script setup>
import { chatPage,chatDelete,chatRecord } from '@/api/chatApi'
import { ref, onMounted,provide,h} from 'vue'
import { tableClickColor } from '@/common/utils.js'
import { tableList,viewList} from '@/api/dataApi'
import page from '../components/Page.vue'
import chatKms from '../components/ChatKms.vue'
import chatBusiness from '../components/ChatBusiness.vue'
import chatMcp from '../components/ChatMcp.vue'
import { ElMessage,ElLoading  } from 'element-plus'

const pageData = ref([]);
const showTitle = ref('');
const propsData = ref(Object);
const isShowChatKms = ref(false);
const isShowBusiness = ref(false);
const isShowMcp = ref(false);

provide("pageEvent",pageEvent);

onMounted(async () => {
    query();  
});

async function pageEvent(data)
{
    await chatPage(data.pageId,data.pageSize).then(res=>{pageData.value=res.data;});  
    tableClickColor('#kmsTable');
}

const query = async () =>{    
    await chatPage(1,10).then(res=>{pageData.value=res.data;});  
    tableClickColor('#kmsTable');    
}

const chatKmsWin = (title)=>
{  
    var data = new Object();
    data.chatRecord=[];
    data.isShow = isShowChatKms;
    data.kms = [];
    data.chatIndex='';
    data.query = query;
    
   propsData.value=data;
   isShowChatKms.value = true;
   showTitle.value=title;
}

const chatMcpWin = (title)=>
{  
    var data = new Object();
    data.chatRecord=[];
    data.isShow = isShowMcp;
    data.mcp = [];
    data.chatIndex='';
    data.query = query;
    
   propsData.value=data;
   isShowMcp.value = true;
   showTitle.value=title;
}

const chatBusinessWin = (title)=>
{
    var data = new Object();
    data.chatIndex='';
    data.tableList=[];
    data.chatRecord=[];
    data.dbInfo={};
    data.isDisabled = false;
    data.isShow = isShowBusiness;
    data.query = query;

   propsData.value=data;
   isShowBusiness.value = true;
   showTitle.value=title;
}

const Delete = async (item)=>
{        
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });    
    let formData = new FormData();
    formData.append('_id', item._id); 
    formData.append('chatIndex', item.chatIndex); 

    await chatDelete(formData).then(res=>{
        loading.close();        
        if(res.data.isSuccess)
            ElMessage.success('删除成功');
        else
            ElMessage.error('删除失败');
    });  
    await chatPage(1,10).then(res=>{pageData.value=res.data;}); 
    tableClickColor('#kmsTable');
}

const Option = async (item)=>
{        
    console.log(item);
    
    showTitle.value='';
    let data = new Object();
    let pageSize = 999;
    data._id = item._id;
    data._index = item._index;
    data.beginTime = item.beginTime;
    data.total = item.total;
    data.chatIndex = item.chatIndex;
    data.name = item.name;
    data.isDisabled = true;
    data.mcp = [];
    data.kms =[];
    
    if (data.chatIndex != '')
        await chatRecord(data.chatIndex).then(res=>{ data.chatRecord = res.data.list;});

    if(item.kms != '' && item.mcp != undefined)
    { 
        JSON.parse(item.kms).forEach(item=>{ 
            data.kms.push(item.vectorIndex);
            showTitle.value += item.name +',';
         });
    }    
    
    if(item.mcp != '' && item.mcp != undefined)
    {         
        JSON.parse(item.mcp).forEach(item=>{             
            data.mcp.push(item._id);
            showTitle.value += item.name +',';
         }); 
    }  

    if(item.isNL2Sql == 'True')
    {        
        data.isShow = isShowBusiness;
        data.dbInfo = JSON.parse(item.dbInfo);
        if(data.dbInfo.isView)
            await viewList(data.dbInfo.key,1,pageSize).then(res=>{ data.tableList = res.data});
        else
            await tableList(data.dbInfo.key,1,pageSize).then(res=>{ data.tableList = res.data});

        isShowBusiness.value = true;
    }
    else if(data.mcp.length>0)
    {
        data.isShow = isShowMcp;
        isShowMcp.value = true;
    }
    else
    {
        data.isShow = isShowChatKms;
        isShowChatKms.value = true;
    }
    propsData.value=data;
}
</script>