/* Offline-compatible subset of Chart.js used by the original Dashboard view.
   It preserves the existing canvas markup/configuration and performs no network I/O. */
window.Chart = class OfflineChart {
  constructor(canvas, config) {
    this.canvas = canvas;
    this.config = config;
    this.resize = () => this.draw();
    this.observer = new ResizeObserver(this.resize);
    this.observer.observe(canvas.parentElement || canvas);
    this.draw();
  }
  destroy() { this.observer.disconnect(); }
  setup() {
    const c = this.canvas, rect = c.getBoundingClientRect(), dpr = window.devicePixelRatio || 1;
    const w = Math.max(280, Math.round(rect.width)), h = Math.max(180, Math.round(rect.height));
    if (c.width !== w * dpr || c.height !== h * dpr) { c.width = w * dpr; c.height = h * dpr; }
    const x = c.getContext("2d"); x.setTransform(dpr, 0, 0, dpr, 0, 0); x.clearRect(0, 0, w, h);
    x.font = '12px "Segoe UI",Tahoma,sans-serif'; x.textBaseline = "middle";
    return { x, w, h };
  }
  draw() { this.config.type === "doughnut" ? this.doughnut() : this.bars(); }
  doughnut() {
    const {x,w,h}=this.setup(), labels=this.config.data.labels||[], ds=this.config.data.datasets?.[0]||{}, vals=ds.data||[];
    const colors=ds.backgroundColor||[], total=vals.reduce((a,v)=>a+(Number(v)||0),0)||1;
    const legendW=Math.min(220,w*.42), cx=(w-legendW)*.46, cy=h*.51, radius=Math.min(h*.33,(w-legendW)*.34), inner=radius*.58;
    let angle=-Math.PI/2;
    vals.forEach((value,i)=>{const next=angle+Math.PI*2*(Number(value)||0)/total;x.beginPath();x.arc(cx,cy,radius,angle,next);x.arc(cx,cy,inner,next,angle,true);x.closePath();x.fillStyle=colors[i]||"#3b82f6";x.fill();angle=next;});
    const row=22, start=Math.max(16,(h-labels.length*row)/2);
    labels.forEach((label,i)=>{const yy=start+i*row, xx=w-legendW+12;x.fillStyle=colors[i]||"#3b82f6";x.fillRect(xx,yy-5,11,11);x.fillStyle="#94a3b8";x.textAlign="left";x.fillText(String(label),xx+19,yy);});
  }
  bars() {
    const {x,w,h}=this.setup(), labels=this.config.data.labels||[], sets=this.config.data.datasets||[];
    const showLegend=this.config.options?.plugins?.legend?.display!==false;
    const top=showLegend?42:18, left=42, right=18, bottom=58, pw=w-left-right, ph=h-top-bottom;
    const values=sets.flatMap(s=>(s.data||[]).map(Number)), rawMax=Math.max(1,...values), step=rawMax<=10?1:Math.ceil(rawMax/5), max=Math.ceil(rawMax/step)*step;
    x.strokeStyle="rgba(148,163,184,.13)";x.lineWidth=1;x.textAlign="right";x.fillStyle="#94a3b8";
    for(let i=0;i<=5;i++){const yy=top+ph*i/5,val=Math.round(max*(5-i)/5);x.beginPath();x.moveTo(left,yy);x.lineTo(w-right,yy);x.stroke();x.fillText(String(val),left-8,yy);}
    if(showLegend){let xx=w-right;[...sets].reverse().forEach(ds=>{const label=String(ds.label||"");const width=x.measureText(label).width+30;xx-=width;x.fillStyle=this.color(ds,0);x.fillRect(xx,14,11,11);x.fillStyle="#94a3b8";x.textAlign="left";x.fillText(label,xx+16,20);});}
    const group=pw/Math.max(1,labels.length), gap=Math.max(2,group*.08), bw=Math.min(42,(group-gap*2)/Math.max(1,sets.length));
    labels.forEach((label,i)=>{const full=sets.length*bw, base=left+i*group+(group-full)/2;sets.forEach((ds,j)=>{const value=Number(ds.data?.[i])||0,bh=ph*value/max,xx=base+j*bw;x.fillStyle=this.color(ds,i);this.roundRect(x,xx+2,top+ph-bh,Math.max(2,bw-4),bh,3);x.fill();});x.fillStyle="#94a3b8";x.textAlign="center";const text=String(label), short=text.length>16?text.slice(0,15)+"…":text;x.fillText(short,left+(i+.5)*group,top+ph+20);});
  }
  color(ds,i){const c=ds.backgroundColor;return Array.isArray(c)?c[i%c.length]:(c||"#3b82f6");}
  roundRect(x,a,b,w,h,r){r=Math.min(r,w/2,h/2);x.beginPath();x.moveTo(a+r,b);x.arcTo(a+w,b,a+w,b+h,r);x.arcTo(a+w,b+h,a,b+h,r);x.arcTo(a,b+h,a,b,r);x.arcTo(a,b,a+w,b,r);x.closePath();}
};
