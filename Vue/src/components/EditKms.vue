<template>
  <div>
    <el-form :model="props.data">      
      <el-form-item label="名称">
        <el-text>{{ Object.keys(props.data)[0] }}</el-text>
      </el-form-item>
      <el-form-item label="内容">
        <el-input type="textarea" v-model="props.data.text" rows="12" />
      </el-form-item>
      <el-button style="margin-left: 400px;" @click="submit" class="btn btn-primary">提交</el-button>
    </el-form>
  </div>
</template>
<script setup>
import { ElMessage,ElLoading} from 'element-plus'
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

const submit = async () =>
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
            ElMessage.success("操作成功");
        else
            ElMessage.error("操作失败");
        props.data.isShow = false;        
    });
}
</script>