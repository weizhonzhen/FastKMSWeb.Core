<template>
    <table class="table table-bordered TableList"> 
        <tbody> 
            <tr>  
                <td align="right" width="10%" valign="middle"><label>名称：</label></td> 
                <td>{{ Object.keys(props.data)[0] }}</td>
            </tr>  
            <tr>  
                <td align="right" width="10%" valign="middle"><label>内容：</label></td> 
                <td>
                    <el-input type="textarea" v-model="props.data.text" rows="12" />
                </td> 
            </tr> 
            <tr> 
                <td align="center" colspan="2"> 
                    <button @click="Submit" class="btn btn-primary">提交</button>
                </td> 
            </tr> 
        </tbody> 
    </table>
</template>
<script setup>
import { ElMessage,ElLoading  ,ElInput} from 'element-plus'
import { defineProps,defineEmits} from 'vue'
import { kmsUpdate} from '@/api/kmsApi'
const props = defineProps({
    data: {
      type: Object,
      required: true,
      default: () => ({
        text: '',
        _index: '',
        _id: '',
        name: '',
        isShow: true
      })
    }
});

async function Submit()
{
    let formData = new FormData();
    formData.append('name', Object.keys(props.data)[0]); 
    formData.append('_id', props.data._id); 
    formData.append('text', props.data.text); 
    formData.append('_index', props.data._index); 

    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });    
    await kmsUpdate(formData).then(res=>{
        loading.close();
        if(res.data.isSuccess)
            ElMessage({message: "操作成功",type: 'success'});
        else
            ElMessage({message: "操作失败",type: 'warning'});
        props.data.isShow = false;        
    });
}
</script>