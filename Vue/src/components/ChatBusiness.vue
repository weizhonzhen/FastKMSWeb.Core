<template>
    <section class="content">
        <div style="height:510px;overflow-y: scroll;" class="box-body">      
            <div v-for="item in props.data.chatRecord" style="clear:both; border: solid 1px #efefef;">
                <div style="background-color: #6CC2CC;border-radius: 5%;float:right;padding:5px;margin-left:10px;">用户</div>
                <div style="background-color: #efefef;border-radius: 1%;float:right;padding:5px;" v-html="item.request" :title="item.vectorContent"></div>
                <div style="background-color: #6CC2CC;border-radius: 5%;clear:both;float:left;padding:5px;margin-right:10px;">{{ item.model }}</div>
                <div style="background-color: #efefef;border-radius: 1%;clear:both;float:left;padding:5px;margin:10px 0px 0px 20px;" v-html="item.response"></div>
            </div>
        </div>
        <div style="height:150px;margin:10px;width:95%;">
            <div><el-Input type="textarea" v-model="message" rows="4" style="resize:none;" :autocomplete="off"></el-Input></div>
            <div style="float:left;margin-top:10px;">            
            <div>
                  <el-select style="width:90px;margin-right:10px;" @change="DbChange" v-model="props.data.dbInfo.key" :disabled="props.data.isDisabled">
                      <el-option v-for="item in dataConfig" :value="item">{{ item }}</el-option>
                  </el-select>        
                  <el-select @change="TypeChanage" style="width:90px;margin-right:10px;" v-model="isView" :disabled="props.data.isDisabled">
                     <el-option
                          v-for="item in options"
                          :key="item.value"
                          :label="item.label"
                          :value="item.value"
                        />
                  </el-select>
                  <el-select style="width:500px;margin-right:10px;" multiple v-model="props.data.dbInfo.tableName" :disabled="props.data.isDisabled">
                     <el-option v-for="item in props.data.tableList.list" :value="item.tabName">{{ item.tabName}}({{ item.tabComments }})</el-option>
                  </el-select> 
            </div>
            </div>
            <div style="padding-top:10px;padding-left:10px;">
                <button class="btn btn-primary btn-sm" @click="Send">发送</button>
                <button class="btn btn-primary btn-sm" style="margin-left:10px;" @click="Close">关闭</button>
            </div>
        </div>
    </section>
</template>

<script setup>
import { defineProps,defineEmits,onMounted,ref} from 'vue'
import { ElSelect,ElOption,ElInput, ElMessage,ElLoading  } from 'element-plus'
import { dataList,tableList,viewList} from '@/api/dataApi'
import { nl2Sql,chatRecord} from '@/api/chatApi'
const props = defineProps({
    data: {
      type: Object,
      required: true
    }
});

const message = ref('');
const options = [
  {
    value: '1',
    label: '表',
  },
  {
    value: '2',
    label: '视图'
  }
]

let pageSize = 999;
const isView = ref('1');

if(!props.data.dbInfo.isView)
  isView.value="1";
else
  isView.value="2";

const dataConfig = ref([]);
onMounted(async () => {
    await dataList().then(res=>{dataConfig.value=res.data;});
}); 

async function Send()
{   
    if(message.value == '')
    {  
        ElMessage({message: "内容不能为空",type: 'warning'});
        return;
    }

    if(props.data.dbInfo.tableName == undefined || props.data.dbInfo.tableName == '' )
    {
        ElMessage({message: "表不能为空",type: 'warning'});
        return;
    }

    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });  

    var data = new Object();
    data.message = message.value;
    data.dbInfo = props.data.dbInfo;
    data.chatIndex = props.data.chatIndex;

    await nl2Sql(data).then(async res=>{
        props.data.chatIndex = res.data.chatIndex;
        await chatRecord(props.data.chatIndex).then(r=>{ props.data.chatRecord = r.data.list;});
        loading.close();
        message.value='';
        props.data.isDisabled=true;
    });
}

function Close()
{
  if(props.data.query!=undefined)
      props.data.query();
   props.data.isShow =false;
}

async function DbChange()
{
  if(props.data.dbInfo.isView)
     await viewList(props.data.dbInfo.key,1,pageSize).then(res=>{ props.data.tableList = res.data});
  else
    await tableList(props.data.dbInfo.key,1,pageSize).then(res=>{ props.data.tableList = res.data});
}

async function TypeChanage()
{  
  if(props.data.dbInfo.Key == undefined)
  {
      ElMessage({message: "数据库不能为空",type: 'warning'});
      return;
  }

  if(isView.value=="2")
     await viewList(props.data.dbInfo.key,1,pageSize).then(res=>{ props.data.tableList = res.data});
  else
    await tableList(props.data.dbInfo.key,1,pageSize).then(res=>{ props.data.tableList = res.data});  
}
</script>