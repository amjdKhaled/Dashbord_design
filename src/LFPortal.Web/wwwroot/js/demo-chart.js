/* Tiny offline canvas renderer for this demo build; intentionally no CDN dependency. */
window.Chart = class DemoChart {
  constructor(canvas, config) { this.canvas = canvas; this.config = config; this.draw(); }
  destroy() {}
  draw() {
    const c=this.canvas, ctx=c.getContext('2d'), box=c.getBoundingClientRect();
    c.width=Math.max(320,box.width*devicePixelRatio); c.height=Math.max(220,box.height*devicePixelRatio);
    ctx.scale(devicePixelRatio,devicePixelRatio); const w=c.width/devicePixelRatio,h=c.height/devicePixelRatio;
    const labels=this.config.data.labels||[], sets=this.config.data.datasets||[], colors=['#3B82F6','#14B8A6','#F59E0B','#8B5CF6','#EF4444','#22C55E','#F97316','#06B6D4'];
    ctx.font='12px Segoe UI'; ctx.fillStyle='#94a3b8';
    if(this.config.type==='doughnut') { const vals=sets[0]?.data||[], total=vals.reduce((a,b)=>a+b,0), cx=w*.32,cy=h/2,r=Math.min(h*.35,w*.2); let a=-Math.PI/2;
      vals.forEach((v,i)=>{const n=a+Math.PI*2*v/total;ctx.beginPath();ctx.arc(cx,cy,r,a,n);ctx.arc(cx,cy,r*.52,n,a,true);ctx.closePath();ctx.fillStyle=colors[i%colors.length];ctx.fill();a=n;ctx.fillStyle='#cbd5e1';ctx.fillText(labels[i],w*.58,24+i*22)}); return; }
    const vals=sets.flatMap(s=>s.data||[]), max=Math.max(1,...vals), group=w/Math.max(1,labels.length), bw=Math.min(34,group/(sets.length+1));
    labels.forEach((label,i)=>{sets.forEach((set,j)=>{const v=set.data[i]||0,bh=(h-55)*v/max;ctx.fillStyle=colors[(i+j)%colors.length];ctx.fillRect(20+i*group+j*bw,h-30-bh,bw-3,bh)});ctx.fillStyle='#94a3b8';ctx.fillText(String(label).slice(0,14),20+i*group,h-10)});
  }
};
