<template>
<section class="content">
    <div class="form-inline">
        <div class="form-group">
            <div class="form-group">
                <button class="btn btn-primary" @click="AddMcp">增加</button>
            </div>
        </div>
    </div>
    <div style="margin-top:10px;" class="box-body table-responsive"> 
        <table id="mcpTable" class="table table-bordered TableList">
            <thead style="background-color:#cacaca;">
                <tr>
                    <td width="10%">名称</td>
                    <td width="10%">备注</td>
                    <td width="25%">参数</td>
                    <td width="10%">方式</td>
                    <td width="25%">地址</td>
                    <td width="10%">操作</td>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in pageData.list">
                    <td>{{ item.name }}</td>
                    <td>{{ item.description }}</td>
                    <td>{{ item.parameters }}</td>
                    <td>{{ item.httpMethod }}</td>
                    <td>{{ item.httpUrl }}</td>
                    <td>
                        <button class="btn btn-primary btn-sm" @click="Delete(item)">删除</button>
                        <button style="margin-left: 10px;" class="btn btn-primary btn-sm" @click="Update(item)">修改</button>
                    </td>
                </tr>
            </tbody>
        </table>
        <page v-model:data="pageData.page" v-model:list="pageData.list"/>
    </div>
</section>
<el-dialog title="增加Mcp" v-model="isShow" :close-on-click-modal="false" :close-on-press-escape ="false" :destroy-on-close="true" :before-close="LoadData">
        <addMcp :data="propsData"/>
</el-dialog>
</template>

<style scoped>
    tabel,tr,td{
        text-align: center;
    }
</style>

<script setup>
import { ElMessage,ElLoading  } from 'element-plus'
import { ref, onMounted,provide,h} from 'vue'
import { mcpPage,mcpDelete,mcpAdd } from '@/api/mcpApi'
import { tableClickColor } from '@/common/utils.js'
import page from '../components/Page.vue'
import addMcp from '../components/AddMcp.vue'

const isShow=ref(false);
const pageData = ref([]);
const propsData = ref(Object);
provide("pageEvent",pageEvent);

onMounted(async () => {
    LoadData();
});   

async function pageEvent(data)
{
    await mcpPage(data.pageId,data.pageSize).then(res=>{pageData.value=res.data;});  
    tableClickColor('#mcpTable');
}

const LoadData = async ()=>
{
    isShow.value = false;
    await mcpPage(1,10).then(res=>{pageData.value=res.data;});  
    tableClickColor('#mcpTable');
}

const Delete = async (item)=>
{    
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });    
    item.parameters=JSON.parse(item.parameters);
    await mcpDelete(item).then(res=>{
        loading.close();
        if(res.data.isSuccess)
            ElMessage.success('删除成功');
        else
            ElMessage.error('删除失败');
    });  
    await mcpPage(1,10).then(res=>{pageData.value=res.data;}); 
    tableClickColor('#mcpTable');
}

const AddMcp=()=>
{    
    let item = new Object();
    item.query=LoadData;
    item.name='';
    item.description='';
    item.parameters=[{ name:'' ,type: '', description: ''}];
    item._id='';
    isShow.value = true;
    item.isShow = isShow;
    item.isAdd=true;
    propsData.value=item; 
}

const Update= (item)=>{
    isShow.value = true;
    item.isShow = isShow;
    item.isAdd=false;
    item.parameters=JSON.parse(item.parameters);
    propsData.value=item;    
}
</script>