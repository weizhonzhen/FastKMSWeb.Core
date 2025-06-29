<template>
    <div class="form-inline">
        <div class="form-group">
            <div class="form-group" style="width:220px">
                <label>数据库</label>
                <select class="form-control" @change="selectChanage" style="width:160px;margin-left: 10px;">
                    <option v-for="item in dataConfig" :value="item">{{ item }}</option>
                </select>
            </div>
            <div class="form-group" style="width:220px;margin-left: 10px;">
                <label>类型</label>
                <select class="form-control" @change="typeChanage" style="width:160px;margin-left: 10px;">
                    <option value="1">表</option>
                    <option value="2">视图</option>
                </select>
            </div>
        </div>
    </div>
    <div style="margin-top:10px;float:left;width:30%;" class="box-body table-responsive">
        <table id="businessTable" class="table table-bordered TableList">
            <thead style="background-color:#cacaca;">
                <tr>
                    <td width="50%">表名</td>
                    <td width="50%">备注</td>
                </tr>
            </thead>
            <tbody>
                <tr v-for="table in pageData.list" @click="showColumn(table)">
                    <td width="50%">{{ table.tabName }}</td>
                    <td width="50%">
                        <el-input type="text" v-model="table.tabComments" autocomplete="off" @blur="updateTable(table)" clearable/>
                    </td>
                </tr>
            </tbody>
        </table>
        <MinPage v-model:page="pageData.page" v-model:list="pageData.list"/>
    </div>
    <div style="margin-top:10px;float:left;width:68%;margin-left:10px;height:800px;overflow-y:scroll;">
        <Column v-model:list="columnData" :table-name="tableName" :is-view="isView" :db-key="key"/>
    </div>
</template>

<style scoped>
    table thead tr td { text-align: center;}
    table tbody tr td {text-align: left;}
</style>

<script setup>
import { ElLoading ,ElInput ,ElMessage} from 'element-plus'
import { ref,provide,onMounted} from 'vue'
import { tableClickColor } from '@/common/utils.js'
import MinPage from '../components/MinPage.vue'
import Column from '../components/Column.vue'
import { tableList,viewList,dataList,columnList,tabUpdate} from '@/api/dataApi'

const pageSize = 10;
let key = '';
let tableName ='';
let isView = false;
const dataConfig = ref([]);
const pageData = ref([]);
const columnData = ref([]);
provide("pageEvent",pageEvent);

onMounted(async () => {
    await dataList().then(res=>{dataConfig.value=res.data;});    
    if(key == '')
        key = dataConfig.value[0];

    query(1,pageSize);
}); 

async function pageEvent(page)
{
    query(page.pageId,page.pageSize);
}

const query = async (pageId,pageSize)=>
{
    if(isView)
        await viewList(key,pageId,pageSize).then(res=>{ pageData.value=res.data});
    else
        await tableList(key,pageId,pageSize).then(res=>{ pageData.value=res.data});
    
    if(pageData.value.list.length>0)
        tableClickColor('#businessTable');
    else
        columnData.value = [];
}

const selectChanage=(event)=>
{
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });   
    key = event.target.value;
    query(1,pageSize);
    loading.close();
}

const typeChanage=(event)=>
{
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });   
    isView = event.target.value== "1" ? false : true;
    query(1,pageSize);
    loading.close();
}

const showColumn = async(item)=>
{
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });   
    tableName = item.tabName;
    await columnList(key,tableName,isView).then(res=>{ columnData.value=res.data});
    loading.close();
}

const updateTable = async(item)=>
  {
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });   
    let data = new Object();
    data.key= key; 
    data.isView=isView; 
    data.tabName=item.tabName; 
    data.tabComments=item.tabComments; 
    await tabUpdate(data).then(res=>{
        loading.close();
        if(res.data.isSuccess)
            ElMessage.success("操作成功");
        else
            ElMessage.error("操作失败");        
    });        
  }
</script>