<template>
<section class="content">
    <div class="form-inline">
        <div class="form-group">
            <div class="form-group">
                <button class="btn btn-primary" @click="AddKms">增加知识库</button>
            </div>
        </div>
    </div>
    <div style="margin-top:10px;" class="box-body table-responsive"> 
        <table id="kmsTable" class="table table-bordered TableList">
            <thead style="background-color:#cacaca;">
                <tr>
                    <td width="15%">名称</td>
                    <td width="10%">时间</td>
                    <td width="35%">备注</td>
                    <td width="10%">操作</td>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in pageData.list">
                    <td><router-link :to="{ path:'kmsList', query: { id: item.vectorIndex }}">{{ item.name }}</router-link ></td>
                    <td>{{ item.dateTime }}</td>
                    <td>{{ item.remark }}</td>
                    <td><button class="btn btn-primary btn-sm" @click="Delete(item)">删除</button></td>
                </tr>
            </tbody>
        </table>
        <page v-model:page="pageData.page" v-model:list="pageData.list"/>
    </div>
</section>
<el-dialog title="增加知识库" v-model="isShow" :close-on-click-modal="false" :close-on-press-escape ="false" :destroy-on-close="true" :before-close="LoadData">
        <addKms/>
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
import { kmsPage,kmsDelete } from '@/api/kmsApi'
import { tableClickColor } from '@/common/utils.js'
import page from '../components/Page.vue'
import addKms from '../components/AddKms.vue'

const isShow=ref(false);
const pageData = ref([]);
provide("pageEvent",pageEvent);

onMounted(async () => {
    LoadData();
});   

async function pageEvent(page)
{
    await kmsPage(page.pageId,page.pageSize).then(res=>{pageData.value=res.data;});  
    tableClickColor('#kmsTable');
}

const LoadData = async ()=>
{
    isShow.value = false;
    await kmsPage(1,10).then(res=>{pageData.value=res.data;});  
    tableClickColor('#kmsTable');
}

const Delete = async (item)=>
{    
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });    
    let formData = new FormData();
    formData.append('vectorIndex', item.vectorIndex); 
    formData.append('index', item.index); 

    await kmsDelete(formData).then(res=>{
        loading.close();
        if(res.data.isSuccess)
            ElMessage({message: '删除成功',type: 'success'});
        else
            ElMessage({message: '删除失败',type: 'warning'});
    });  
    await kmsPage(1,10).then(res=>{pageData.value=res.data;}); 
    tableClickColor('#kmsTable');
}

const AddKms=()=>
{    
    isShow.value=true;
}
</script>