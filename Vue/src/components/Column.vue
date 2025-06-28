<template>
    <table id="columnTable" class="table table-bordered TableList">
    <thead style="background-color:#cacaca;position: sticky;position: -webkit-sticky;top: 0;">
        <tr>
            <td  width="20%">列名</td>
            <td  width="10%">主键</td>
            <td  width="20%">类型</td>
            <td  width="50%">备注</td>
        </tr>
    </thead>
    <tbody>
        <tr v-for=" colmun in props.list">
            <td width="20%">{{ colmun.colName }}</td>
            <td width="10%">{{ colmun.isKey?'是':'否' }}</td>
            <td width="20%">{{ colmun.showType }}</td>
            <td width="50%">
                <el-input type="text" v-model="colmun.colComments" autocomplete="off" @blur="UpdateColumn(colmun)" clearable/>
            </td>
        </tr>
    </tbody>
</table>
</template>

<style scoped>
    table tr td { text-align: center;}
</style>

<script setup>
import { ElMessage,ElLoading,ElInput  } from 'element-plus'
import { defineProps,onUpdated} from 'vue'
import { tableClickColor } from '@/common/utils.js'
import { colUpdate} from '@/api/dataApi'

const props = defineProps({    
    list:{
      type: Object,
      required: true,
      default: () => ({
        colName: '',
        isKey: Boolean,
        showType: '',
        colComments: ''
      })
    },
    tableName:{
        type: String,
        default: ''
    },
    isView:{
        type: Boolean,
        default: false
    },
    dbKey:{
        type: String,
        default: ''
    }  
});

onUpdated(() => {
    if(props.list.length>0)
        tableClickColor('#columnTable');
}); 

async function UpdateColumn(item)
{
    if(item.colComments == '')
    {
        ElMessage({message: '备注不能为空',type: 'warning'});
        return;
    }
    
    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });    
    
    let formData = new FormData();
    formData.append('key', props.dbKey); 
    formData.append('isView', props.isView); 
    formData.append('tableName', props.tableName); 

    formData.append('colName', item.colName); 
    formData.append('colComments', item.colComments); 
    formData.append('colType', item.colType); 
    formData.append('showType', item.showType);        

    await colUpdate(formData).then(res=>{
        loading.close();
        if(res.data.isSuccess)
                ElMessage({message: "操作成功",type: 'success'});
            else
                ElMessage({message: "操作失败",type: 'warning'});
        });
}
</script>