<template>
<section class="content">
    <div class="form-inline">
        <div class="form-group">
            <div class="form-group" style="width:320px">
                <label>知识库</label>
                <select class="form-control" v-model="selectedValue" v-on:change="selectChanage" id="kmsSelect"  style="width:160px;">
                    <option value="">请选择</option>
                    <option v-for="item in kmsData" :value='item.vectorIndex'>{{ item.name }}</option>
                </select>
            </div>
        </div>
    </div>

    <div style="margin-top:10px;" class="box-body table-responsive">
        <table id="kmsTable" class="table table-bordered TableList">
            <thead style="background-color:#cacaca;">
                <tr>
                    <td width="10%">知识库</td>
                    <td width="80%">内容</td>
                    <td width="10%">操作</td>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in pageData.list">
                    <td width="10%" style="word-break:break-word">{{ Object.keys(item)[0] }}</td>
                    <td width="80%" style="word-break:break-word">{{ item.text }}</td>
                    <td width="10%">
                        <button class="btn btn-primary btn-sm" @click="Update(item)">修改</button>
                        <button class="btn btn-primary btn-sm" @click="Delete(item)">删除</button>
                    </td>
                </tr>
            </tbody>
        </table>
        <page v-model:page="pageData.page" v-model:list="pageData.list"/>
    </div>
</section>     
<el-dialog title="修改知识库" v-model="isShow" :close-on-click-modal="false" :destroy-on-close="true" :close-on-press-escape ="false">
    <editKms :data="propsData"/>
</el-dialog>
</template>

<style scoped>
    tabel,tr,td{
        text-align: center;
    }
   select, button {margin-left: 10px;}
   select {width: 160px;}
</style>

<script setup>
import { ElMessage,ElLoading  } from 'element-plus'
import { useRoute } from 'vue-router';
import { ref, onMounted ,provide, h} from 'vue'
import { vectorPage,vectorDelete } from '@/api/vectorApi'
import { kmsList } from '@/api/kmsApi'
import { tableClickColor } from '@/common/utils.js'
import page from '../components/Page.vue'
import editKms from '../components/EditKms.vue'

const route = useRoute();
const propsData = ref(Object);
const isShow=ref(false);
const pageData = ref([]);
const kmsData = ref([]);
const selectedValue = ref('');
let vectorIndex = [];
let pageSize = 8;

provide("pageEvent",pageEvent);

onMounted(async () => {
    await kmsList().then(res=>{kmsData.value=res.data});
    Object.entries(kmsData.value).map(([key,value]) => {
        vectorIndex.push(value.vectorIndex);
    });
    let id = route.query.id;
    if(id!=undefined)
    {
        vectorIndex=[];
        selectedValue.value = id;
        vectorIndex.push(id);
        await vectorPage(id,1,pageSize).then(res=>{pageData.value=res.data;});
    }
    else
        await vectorPage(vectorIndex.join(),1,pageSize).then(res=>{pageData.value=res.data;});
    tableClickColor('#kmsTable');   
});   

async function pageEvent(page)
{
    await vectorPage(vectorIndex.join(),page.pageId,page.pageSize).then(res=>{pageData.value=res.data;});  
    tableClickColor('#kmsTable');
}

const selectChanage = async (event) =>
{        
    if(event.target.value !='')
    {
        vectorIndex=[];
        vectorIndex.push(event.target.value);
    }
    else{
        Object.entries(kmsData.value).map(([key,value]) => {
            vectorIndex.push(value.vectorIndex);
        });
    }
    
    await vectorPage(vectorIndex.join(),1,pageSize).then(res=>{pageData.value=res.data;});  
    tableClickColor('#kmsTable');
}

const Delete = async (item)=>
{       
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });
    
    let formData = new FormData();
    formData.append('index', item._index); 
    formData.append('id', item._id); 

    await vectorDelete(formData).then(res=>{
        loading.close();
        if(res.data.isSuccess)
            ElMessage({message: '删除成功',type: 'success'});
        else
            ElMessage({message: '删除失败',type: 'warning'});
    });      
    await vectorPage(vectorIndex.join(),1,pageSize).then(res=>{pageData.value=res.data;});  
    tableClickColor('#kmsTable');
}

const Update = (item) =>
{   
    isShow.value = true;
     item.isShow = isShow;
    propsData.value=item;    
}
</script>